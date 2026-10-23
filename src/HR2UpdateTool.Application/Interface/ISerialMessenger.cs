using HR2UpdateTool.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.Interface
{
    public interface ISerialMessenger
    {
        Task SendCommandAsync(byte cmd, byte[]? payload = null);
        Task<SerialPacket> SendCommandAndAwaitResponseAsync(
            byte cmd,
            byte[]? payload = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        Task SendRawDataAsync(byte[] packet);
        Task<byte[]> SendRawDataAndAwaitResponseAsync(
            byte[] rawData,
            Func<List<byte>, bool> isExpectedResponse,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        event EventHandler<SerialPacketEventArgs>? PacketReceived;
    }
}
