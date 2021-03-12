using System;
using System.Collections.Generic;
using System.Text;
using DtronixHash.MurMur3;

namespace DtronixHash
{
    public class NcHashBuffer
    {
        private readonly NcHashAlgorithm _algorithm;
        private readonly Memory<byte> _buffer;
        private int _bufferPosition;
        private readonly int _blockSizeBytes;

        public NcHashBuffer(NcHashAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _blockSizeBytes = algorithm.InputBlockSize / 8;
            _buffer = new Memory<byte>(new byte[_blockSizeBytes]);
        }

        public void Write(ReadOnlyMemory<byte> data)
        {
            var remainder = data.Length & 15;
            var blocks = data.Length / 16;

            // Hot path.  Buffer is empty.
            if (_bufferPosition == 0)
            {
                if (remainder == 0 && blocks > 0)
                {
                    // Hot hot path
                    _algorithm.TransformBlock(data);
                }
                else
                {
                    // We don't have enough for hashing, so buffer for next call.
                    data.Slice(0, remainder).CopyTo(_buffer);
                    _bufferPosition = data.Length;
                }
            }
            else
            {
                var maxFill = 16 - _bufferPosition;

                // If we have enough data to transform now fill the buffer
                var bufferFillLength = Math.Min(maxFill, data.Length);

                // Copy what can be filled into the buffer
                data.Slice(0, bufferFillLength).CopyTo(_buffer.Slice(_bufferPosition));
                _bufferPosition += bufferFillLength;

                // determine if we have enough to 
                if (_bufferPosition == 16)
                {
                    // Transform the buffer.
                    _algorithm.TransformBlock(_buffer);
                    _bufferPosition = 0;
                }

                // Pass on the rest of the data to be buffered or transformed.
                if (data.Length > bufferFillLength)
                    Write(data.Slice(bufferFillLength));
            }
        }

        public Memory<byte> FinalizeHash()
        {
            if(_bufferPosition > 0)
                _algorithm.TransformBlock(_buffer.Slice(0, _bufferPosition));

            _bufferPosition = 0;

            return _algorithm.FinalizeHash();
        }
    }
}
