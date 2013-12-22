using System;
using System.IO;
using System.Text;

namespace UpdateControls.Correspondence.FieldSerializer
{
    public static class BinaryHelper
    {
        // Don't include byte order mark, and throw on invalid encodings.
        public static readonly UTF8Encoding Encoding = new UTF8Encoding(false, true);

        public static void WriteByte(byte value, BinaryWriter output)
        {
            output.Write(value);
        }

        public static byte ReadByte(BinaryReader input)
        {
            return input.ReadByte();
        }

        public static void WriteChar(char value, BinaryWriter output)
        {
            byte[] data = Encoding.GetBytes(new char[] { value });
            output.Write(data);
        }

        public static char ReadChar(BinaryReader input)
        {
            byte firstByte = input.ReadByte();
            byte[] data;
            if ((firstByte & 0x80) == 0)
                data = new byte[1] { firstByte };
            else if ((firstByte & 0xe0) == 0xc0)
            {
                byte secondByte = input.ReadByte();
                data = new byte[2] { firstByte, secondByte };
            }
            else if ((firstByte & 0xf0) == 0xe0)
            {
                byte secondByte = input.ReadByte();
                byte thirdByte = input.ReadByte();
                data = new byte[3] { firstByte, secondByte, thirdByte };
            }
            else if ((firstByte & 0xf8) == 0xf0)
            {
                byte secondByte = input.ReadByte();
                byte thirdByte = input.ReadByte();
                byte fourthByte = input.ReadByte();
                data = new byte[4] { firstByte, secondByte, thirdByte, fourthByte };
            }
            else if ((firstByte & 0xfc) == 0xf8)
            {
                byte secondByte = input.ReadByte();
                byte thirdByte = input.ReadByte();
                byte fourthByte = input.ReadByte();
                byte fifthByte = input.ReadByte();
                data = new byte[5] { firstByte, secondByte, thirdByte, fourthByte, fifthByte };
            }
            else
            {
                byte secondByte = input.ReadByte();
                byte thirdByte = input.ReadByte();
                byte fourthByte = input.ReadByte();
                byte fifthByte = input.ReadByte();
                byte sixthByte = input.ReadByte();
                data = new byte[6] { firstByte, secondByte, thirdByte, fourthByte, fifthByte, sixthByte };
            }
            return Encoding.GetChars(data, 0, data.Length)[0];
        }

        public static void WriteBoolean(bool value, BinaryWriter output)
        {
            byte data = value ? (byte)1 : (byte)0;
            output.Write(data);
        }

        public static bool ReadBoolean(BinaryReader input)
        {
            byte data = input.ReadByte();
            return data != 0;
        }

        public static void WriteShort(short value, BinaryWriter output)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                data = ReverseBytes(data);
            output.Write(data);
        }

        public static short ReadShort(BinaryReader input)
        {
            byte[] data = input.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                data = ReverseBytes(data);
            return BitConverter.ToInt16(data, 0);
        }

        public static void WriteInt(int value, BinaryWriter output)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                data = ReverseBytes(data);
            output.Write(data);
        }

        public static int ReadInt(BinaryReader input)
        {
            byte[] data = input.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                data = ReverseBytes(data);
            return BitConverter.ToInt32(data, 0);
        }

        public static void WriteLong(long value, BinaryWriter output)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                data = ReverseBytes(data);
            output.Write(data);
        }

        public static long ReadLong(BinaryReader input)
        {
            byte[] data = input.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                data = ReverseBytes(data);
            return BitConverter.ToInt64(data, 0);
        }

        public static string ReadString(BinaryReader input)
        {
            short length = ReadShort(input);
            if (length > Model.MaxDataLength)
                throw new CorrespondenceException(string.Format("String length {0} exceeded the maximum bytes allowable ({1}).", length, Model.MaxDataLength));
            if (length == 0)
                return string.Empty;

            byte[] data = input.ReadBytes(length);
            string value = Encoding.GetString(data, 0, length);
            return value;
        }

        public static void WriteString(string value, BinaryWriter output)
        {
            string str = (string)value;
            if (string.IsNullOrEmpty(str))
                WriteShort(0, output);
            else
            {
                byte[] data = Encoding.GetBytes(str);
                int length = data.Length;
                if (length > Model.MaxDataLength)
                    throw new CorrespondenceException(string.Format("String length {0} exceeded the maximum bytes allowable ({1}).", length, Model.MaxDataLength));
                WriteShort((short)length, output);
                output.Write(data);
            }
        }

        public static byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }
    }
}
