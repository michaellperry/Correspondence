using System;
using System.IO;

namespace UpdateControls.Correspondence.Data
{
    public abstract class StreamBasedDataStructure : IDisposable
    {
        private byte[] _readBuffer = new byte[8];
        protected Stream _stream;

        protected StreamBasedDataStructure(Stream stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        protected void SeekTo(long position)
        {
            _stream.Seek(position, SeekOrigin.Begin);
        }

        protected byte ReadByte()
        {
            return (byte)_stream.ReadByte();
        }

        protected void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        protected int ReadInt()
        {
            _stream.Read(_readBuffer, 0, 4);
            return BitConverter.ToInt32(_readBuffer, 0);
        }

        protected void WriteInt(int value)
        {
            byte[] writeBuffer = BitConverter.GetBytes(value);
            _stream.Write(writeBuffer, 0, 4);
        }

        protected long ReadLong()
        {
            _stream.Read(_readBuffer, 0, 8);
            return BitConverter.ToInt64(_readBuffer, 0);
        }

        protected void WriteLong(long value)
        {
            byte[] writeBuffer = BitConverter.GetBytes(value);
            _stream.Write(writeBuffer, 0, 8);
        }

        protected void ReadBytes(byte[] data)
        {
            _stream.Read(data, 0, data.Length);
        }

        protected void WriteBytes(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }

        public long GetSize()
        {
            return _stream.Length;
        }
    }
}
