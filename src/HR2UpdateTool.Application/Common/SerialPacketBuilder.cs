using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.Common
{
    public static class SerialPacketBuilder
    {
        public static byte[] Build(byte cmd, byte[]? payload = null)
        {
            payload ??= Array.Empty<byte>();
            ushort payloadLength = (ushort)payload.Length;

            int totalLength = 5 + payloadLength + 1;
            byte[] frame = new byte[totalLength];

            frame[0] = CommandConstants.STX1;
            frame[1] = CommandConstants.STX2;

            frame[2] = cmd;

            frame[3] = (byte)(payloadLength & 0xFF);
            frame[4] = (byte)((payloadLength >> 8) & 0xFF);

            if (payloadLength > 0)
            {
                Array.Copy(payload, 0, frame, 5, payloadLength);
            }

            byte checksum = GetChecksum(cmd, payload);
            frame[totalLength - 1] = checksum;

            return frame;
        }

        public static byte GetChecksum(byte cmd, byte[] payload)
        {
            int sum = (payload).Sum(b => b);
            byte checksum = (byte)(sum & 0xFF);
            return (byte)(cmd + (byte)(payload.Length & 0xFF) + (byte)(payload.Length >> 8) + checksum);
        }
    }
}
