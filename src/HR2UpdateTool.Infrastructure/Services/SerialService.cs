using HR2UpdateTool.Application.Interface;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Infrastructure.Services
{
    public class SerialService : ISerialService, IDisposable
    {
        private SerialPort? _serialPort;
        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public string? CurrentPortName => IsConnected ? _serialPort?.PortName : null;

        public event EventHandler<byte[]>? DataReceived;
        public event EventHandler<bool>? ConnectionStatusChanged;

        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames().Distinct().OrderBy(p => p).ToArray();
        }

        public void Connect(string portName, int baudRate = 115200)
        {
            if (IsConnected)
            {
                Disconnect();
            }

            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.DataReceived += OnSerialDataReceived;
            _serialPort.Open();

            ConnectionStatusChanged?.Invoke(this, true);
        }

        public void Disconnect()
        {
            if (_serialPort != null)
            {
                try
                {
                    _serialPort.DataReceived -= OnSerialDataReceived;

                    if (_serialPort.IsOpen)
                    {
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();
                        _serialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SerialService] Disconnect error: {ex.Message}");
                }
                finally
                {
                    _serialPort.Dispose();
                    _serialPort = null;
                    ConnectionStatusChanged?.Invoke(this, false);
                }
            }
        }

        public async Task SendDataAsync(byte[] data)
        {
            if (!IsConnected || _serialPort == null)
                throw new InvalidOperationException("Serial port is not connected.");

            await _serialPort.BaseStream.WriteAsync(data, 0, data.Length);
            await _serialPort.BaseStream.FlushAsync();
        }

        public async Task SendDataAsync(string data)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);
            await SendDataAsync(bytes);
        }

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen) return;

            try
            {
                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead <= 0) return;

                byte[] buffer = new byte[bytesToRead];
                int readBytes = _serialPort.Read(buffer, 0, bytesToRead);

                if (readBytes > 0)
                {
                    DataReceived?.Invoke(this, buffer);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SerialService] Read error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
