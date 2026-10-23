using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Infrastructure.Utils
{
    public static class BigEndianBitConverter
    {
        public static byte[] GetBytes(ushort value)

        {

            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)

                Array.Reverse(bytes);

            return bytes;

        }


        public static byte[] GetBytes(short value)

        {

            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)

                Array.Reverse(bytes);

            return bytes;

        }



        public static byte[] GetBytes(uint value)

        {

            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)

                Array.Reverse(bytes);

            return bytes;

        }



        public static byte[] GetBytes(int value)

        {

            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)

                Array.Reverse(bytes);

            return bytes;

        }



        public static ushort ToUInt16(byte[] bytes, int startIndex = 0)

        {

            byte[] temp = bytes.Skip(startIndex).Take(2).ToArray();

            if (BitConverter.IsLittleEndian)

                Array.Reverse(temp);

            return BitConverter.ToUInt16(temp, 0);

        }



        public static uint ToUInt32(byte[] bytes, int startIndex = 0)

        {

            byte[] temp = bytes.Skip(startIndex).Take(4).ToArray();

            if (BitConverter.IsLittleEndian)

                Array.Reverse(temp);

            return BitConverter.ToUInt32(temp, 0);

        }



        public static short ToInt16(byte[] bytes, int startIndex = 0)

        {

            byte[] temp = bytes.Skip(startIndex).Take(2).ToArray();

            if (BitConverter.IsLittleEndian)

                Array.Reverse(temp);

            return BitConverter.ToInt16(temp, 0);

        }



        public static int ToInt32(byte[] bytes, int startIndex = 0)

        {

            byte[] temp = bytes.Skip(startIndex).Take(4).ToArray();

            if (BitConverter.IsLittleEndian)

                Array.Reverse(temp);

            return BitConverter.ToInt32(temp, 0);

        }
    }
}
