using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using BufferAlias = Windows.Storage.Streams.Buffer;

namespace Correspondence.Data
{
    public abstract class StreamBasedDataStructure : IDisposable
    {
        private IRandomAccessStream _randomAccessStream;

        private BufferAlias _byteBuffer = new BufferAlias(1);
        private BufferAlias _intBuffer = new BufferAlias(4);
        private BufferAlias _longBuffer = new BufferAlias(8);
        public StreamBasedDataStructure(IRandomAccessStream randomAccessStream)
        {
            _randomAccessStream = randomAccessStream;
        }

        public void Dispose()
        {
            _randomAccessStream.Dispose();
        }

        public ulong GetSize()
        {
            return _randomAccessStream.Size;
        }

        protected bool Empty()
        {
            return _randomAccessStream.Size == 0;
        }

        protected void SeekTo(long position)
        {
            _randomAccessStream.Seek((ulong)position);
        }

        protected long SeekToEnd()
        {
            ulong end = _randomAccessStream.Size;
            _randomAccessStream.Seek(end);
            return (long)end;
        }

        protected async Task<byte> ReadByteAsync()
        {
            await _randomAccessStream.ReadAsync(_byteBuffer, 1, InputStreamOptions.Partial);
            return _byteBuffer.GetByte(0);
        }

        protected async Task WriteByteAsync(byte value)
        {
            var buffer = new byte[] { value }.AsBuffer();
            await _randomAccessStream.WriteAsync(buffer);
        }

        protected async Task<int> ReadIntAsync()
        {
            await _randomAccessStream.ReadAsync(_intBuffer, 4, InputStreamOptions.Partial);
            var array = _intBuffer.ToArray();
            return BitConverter.ToInt32(array, 0);
        }

        protected async Task WriteIntAsync(int value)
        {
            byte[] array = BitConverter.GetBytes(value);
            var buffer = array.AsBuffer();
            await _randomAccessStream.WriteAsync(buffer);
        }

        protected async Task<long> ReadLongAsync()
        {
            await _randomAccessStream.ReadAsync(_longBuffer, 8, InputStreamOptions.Partial);
            var array = _longBuffer.ToArray();
            return BitConverter.ToInt64(array, 0);
        }

        protected async Task WriteLongAsync(long value)
        {
            byte[] array = BitConverter.GetBytes(value);
            var buffer = array.AsBuffer();
            await _randomAccessStream.WriteAsync(buffer);
        }

        protected async Task ReadBytesAsync(byte[] data)
        {
            BufferAlias buffer = new BufferAlias((uint)data.Length);
            await _randomAccessStream.ReadAsync(buffer, (uint)data.Length, InputStreamOptions.Partial);
            buffer.CopyTo(data);
        }

        protected async Task WriteBytesAsync(byte[] data)
        {
            var buffer = data.AsBuffer();
            await _randomAccessStream.WriteAsync(buffer);
        }

        protected async Task FlushAsync()
        {
            await _randomAccessStream.FlushAsync();
        }
    }
}
