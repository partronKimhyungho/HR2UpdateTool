using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.Common
{
    public class SerialPacket
    {
        public byte Command { get; }
        public byte[] Payload { get; }

        public SerialPacket(byte command, byte[]? payload = null)
        {
            Command = command;
            Payload = payload ?? Array.Empty<byte>();
        }
    }
}
