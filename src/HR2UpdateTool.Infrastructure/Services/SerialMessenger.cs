using HR2UpdateTool.Application.Common;
using HR2UpdateTool.Application.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Infrastructure.Services
{
    public class SerialMessenger : ISerialMessenger, IDisposable
    {
        private readonly ISerialService _serialService;

        public event EventHandler<SerialPacketEventArgs>? PacketReceived;

        private readonly ConcurrentDictionary<byte, TaskCompletionSource<SerialPacket>> _pendingRequests = new();

        private TaskCompletionSource<byte[]>? _rawResponseTcs;
        private Func<List<byte>, bool>? _rawResponsePredicate;

        private readonly List<byte> _buffer = new();
        private readonly object _bufferLock = new();

        public SerialMessenger(ISerialService serialService)
        {
            _serialService = serialService;

            _serialService.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object? sender, byte[] data)
        {
            lock (_bufferLock)
            {
                _buffer.AddRange(data);

                try
                {
                    if (_rawResponseTcs != null &&
                        _rawResponsePredicate != null &&
                        _rawResponsePredicate(_buffer))
                    {
                        _rawResponseTcs.TrySetResult(_buffer.ToArray());

                        _rawResponseTcs = null;
                        _rawResponsePredicate = null;
                    }

                    ProcessBuffer();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SerialMessenger] 패킷 파싱 에러: {ex.Message}");
                    _buffer.Clear();
                }
            }
        }

        private void ProcessBuffer()
        {
            while (_buffer.Count >= 6)
            {
                int stxIndex = -1;
                for (int i = 0; i < _buffer.Count - 1; i++)
                {
                    if (_buffer[i] == CommandConstants.STX1 && _buffer[i + 1] == CommandConstants.STX2)
                    {
                        stxIndex = i;
                        break;
                    }
                }

                if (stxIndex == -1)
                {
                    byte lastByte = _buffer[^1];
                    _buffer.Clear();
                    if (lastByte == CommandConstants.STX1) _buffer.Add(lastByte);
                    break;
                }

                if (stxIndex > 0) _buffer.RemoveRange(0, stxIndex);
                if (_buffer.Count < 5) break;

                int payloadLength = _buffer[3] | (_buffer[4] << 8);
                int fullPacketLength = 5 + payloadLength + 1;

                if (fullPacketLength > 1024)
                {
                    _buffer.RemoveAt(0);
                    continue;
                }

                if (_buffer.Count < fullPacketLength) break;

                byte[] frame = _buffer.GetRange(0, fullPacketLength).ToArray();
                _buffer.RemoveRange(0, fullPacketLength);

                byte cmd = frame[2];
                byte[] payload = new byte[payloadLength];
                if (payloadLength > 0)
                {
                    Array.Copy(frame, 5, payload, 0, payloadLength);
                }

                byte checksum = SerialPacketBuilder.GetChecksum(cmd, payload);

                if (checksum != frame[^1])
                {
                    throw new Exception("Checksum 불일치");
                    Debug.WriteLine("[SerialMessenger] Checksum 불일치 패킷 폐기");
                    continue;
                }

                var packet = new SerialPacket(cmd, payload);

                if (_pendingRequests.TryRemove(cmd, out var tcs))
                {
                    tcs.TrySetResult(packet);
                }

                PacketReceived?.Invoke(this, new SerialPacketEventArgs(packet));
            }
        }

        public async Task SendCommandAsync(byte cmd, byte[]? payload = null)
        {
            byte[] frame = SerialPacketBuilder.Build(cmd, payload);
            await _serialService.SendDataAsync(frame);
        }

        public async Task<SerialPacket> SendCommandAndAwaitResponseAsync(byte cmd, byte[]? payload = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            var timeoutValue = timeout ?? TimeSpan.FromSeconds(3);
            var tcs = new TaskCompletionSource<SerialPacket>(TaskCreationOptions.RunContinuationsAsynchronously);

            if (_pendingRequests.TryRemove(cmd, out var oldTcs))
            {
                oldTcs.TrySetCanceled();
            }

            _pendingRequests[cmd] = tcs;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeoutValue);

            using var registration = cts.Token.Register(() =>
            {
                if (_pendingRequests.TryRemove(cmd, out var removedTcs))
                {
                    removedTcs.TrySetException(new TimeoutException($"Command (0x{cmd:X2}) 응답 시간 초과 ({timeoutValue.TotalSeconds}초)"));
                }
            });

            try
            {
                byte[] frame = SerialPacketBuilder.Build(cmd, payload);
                await _serialService.SendDataAsync(frame);

                return await tcs.Task;
            }
            finally
            {
                _pendingRequests.TryRemove(cmd, out _);
            }
        }

        public async Task SendRawDataAsync(byte[] data)
        {
            await _serialService.SendDataAsync(data);
        }

        public async Task<byte[]> SendRawDataAndAwaitResponseAsync(byte[]rawData, Func<List<byte>, bool> isExpectedResponse, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            var timeoutValue = timeout ?? TimeSpan.FromSeconds(3);

            var tcs = new TaskCompletionSource<byte[]>(
               TaskCreationOptions.RunContinuationsAsynchronously);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            cts.CancelAfter(timeoutValue);

            lock (_bufferLock)
            {
                _buffer.Clear();
                _rawResponseTcs = tcs;
                _rawResponsePredicate = isExpectedResponse;
            }

            using var registration = cts.Token.Register(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.TrySetCanceled(cancellationToken);
                }
                else
                {
                    tcs.TrySetException(
                        new TimeoutException(
                            $"Raw 응답 시간 초과 ({timeoutValue.TotalSeconds}초)"));
                }
            });

            try
            {
                await _serialService.SendDataAsync(rawData);

                return await tcs.Task;
            }
            finally
            {
                lock (_bufferLock)
                {
                    if (ReferenceEquals(_rawResponseTcs, tcs))
                    {
                        _rawResponseTcs = null;
                        _rawResponsePredicate = null;
                    }
                }
            }
        }
        public void Dispose()
        {
            _serialService.DataReceived -= OnDataReceived;
        }
    }
}
