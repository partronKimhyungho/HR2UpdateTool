using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.Common
{
    public class SerialPacketEventArgs : EventArgs
    {
        public SerialPacket Packet { get; }

        public SerialPacketEventArgs(SerialPacket packet)
        {
            Packet = packet;
        }
    }
}
