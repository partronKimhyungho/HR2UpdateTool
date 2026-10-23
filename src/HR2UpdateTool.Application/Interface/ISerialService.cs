using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.Interface
{
    public interface ISerialService
    {
        bool IsConnected { get; }
        string? CurrentPortName { get; }

        event EventHandler<byte[]>? DataReceived;

        event EventHandler<bool>? ConnectionStatusChanged;

        string[] GetAvailablePorts();

        public void Connect(string portName, int baudRate = 115200);
        void Disconnect();

        Task SendDataAsync(byte[] data);

        Task SendDataAsync(string data);
    }
}
