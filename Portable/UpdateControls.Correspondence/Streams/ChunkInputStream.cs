using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UpdateControls.Correspondence.Streams
{
    public static class ChunkInputStream
    {
        public static ChunkInputStream<T> Open<T>(IEnumerable<T> chunks, Func<T, byte[]> bytes)
            where T : CorrespondenceFact
        {
            return new ChunkInputStream<T>(chunks.ToArray(), bytes);
        }
    }

    public class ChunkInputStream<T> : Stream
        where T : CorrespondenceFact
    {
        private readonly T[] _chunks;
        private readonly Func<T, byte[]> _bytes;

        private int _currentChunk;
        private int _bufferPosition;
        private long _streamPosition;
        
        public ChunkInputStream(T[] chunks, Func<T, byte[]> bytes)
        {
            _chunks = chunks;
            _bytes = bytes;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { return _chunks.Sum(c => (long)_bytes(c).Length); }
        }

        public override long Position
        {
            get { return _streamPosition; }
            set { throw new NotImplementedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            while (count > 0 && _currentChunk < _chunks.Length)
            {
                byte[] data = _bytes(_chunks[_currentChunk]);
                int bytesToCopy = Math.Min(count, data.Length - _bufferPosition);

                Buffer.BlockCopy(data, _bufferPosition, buffer, offset, bytesToCopy);
                bytesRead += bytesToCopy;
                _bufferPosition += bytesToCopy;
                offset += bytesToCopy;
                _streamPosition += bytesToCopy;
                count -= bytesToCopy;

                if (_bufferPosition == data.Length)
                {
                    _currentChunk++;
                    _bufferPosition = 0;
                }
            }
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
