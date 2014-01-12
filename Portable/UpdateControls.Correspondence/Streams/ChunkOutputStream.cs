using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.Streams
{
    public static class ChunkOutputStream
    {
        public static ChunkOutputStream<T> Open<T>(ICommunity community, Func<byte[], T> ctor)
            where T : CorrespondenceFact
        {
            return new ChunkOutputStream<T>(community, ctor);
        }
    }

    public class ChunkOutputStream<T> : Stream
        where T : CorrespondenceFact
    {
        private readonly ICommunity _community;
        private readonly Func<byte[], T> _ctor;

        private List<Task<T>> _chunks = new List<Task<T>>();
        private byte[] _buffer = new byte[1022];
        private int _bufferPosition = 0;
        private long _streamPosition = 0;
        
        public ChunkOutputStream(ICommunity community, Func<byte[], T> ctor)
        {
            _community = community;
            _ctor = ctor;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { return _streamPosition; }
        }

        public override long Position
        {
            get { return _streamPosition; }
            set { throw new NotImplementedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
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
            while (count > 0)
            {
                int remaining = Math.Min(count, _buffer.Length - _bufferPosition);

                Buffer.BlockCopy(buffer, offset, _buffer, _bufferPosition, remaining);
                _bufferPosition += remaining;
                offset += remaining;
                count -= remaining;

                if (_bufferPosition == _buffer.Length)
                {
                    NextChunk();
                }
            }
        }

        public Task<T[]> GetChunks()
        {
            NextChunk();
            return Task.WhenAll(_chunks);
        }

        private void NextChunk()
        {
            if (_bufferPosition > 0)
            {
                byte[] subBuffer = new byte[_bufferPosition];
                Buffer.BlockCopy(_buffer, 0, subBuffer, 0, _bufferPosition);
                var chunkTask = _community.AddFactAsync(_ctor(subBuffer));
                _chunks.Add(chunkTask);
                _bufferPosition = 0;
            }
        }
    }
}
