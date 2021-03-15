using System;

namespace DtronixHash
{
    /// <summary>
    /// Non Cryptographic Hash Buffer.
    /// </summary>
    public class NcHashBuffer
    {
        private readonly NcHashAlgorithm _algorithm;
        private readonly Memory<byte> _buffer;
        private int _bufferPosition;
        private readonly int _blockSizeBytes;

        /// <summary>
        /// Algorithm used by the buffer.
        /// </summary>
        public NcHashAlgorithm Algorithm => _algorithm;

        /// <summary>
        /// Creates a new instance of the Non Cryptographic Hash Buffer.
        /// </summary>
        /// <param name="algorithm">Algorithm to use while hashing.</param>
        public NcHashBuffer(NcHashAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _blockSizeBytes = algorithm.InputBlockSize / 8;
            _buffer = new Memory<byte>(new byte[_blockSizeBytes]);
        }

        /// <summary>
        /// Writes data to the managed hash algorithm.
        /// </summary>
        /// <param name="data">Data to hash.</param>
        public void Write(ReadOnlyMemory<byte> data)
        {
            var remainder = data.Length & (_blockSizeBytes - 1);
            var blocks = data.Length / _blockSizeBytes;

            // Hot path.  Buffer is empty.
            if (_bufferPosition == 0)
            {
                if (remainder == 0 && blocks > 0)
                {
                    // Hot hot path
                    _algorithm.TransformBlock(data);
                }
                else if (remainder > 0 && blocks > 0)
                {
                    // We have excess and blocks to transform now.
                    var transformLength = data.Length - remainder;
                    _algorithm.TransformBlock(data.Slice(0, transformLength));

                    // Store for next call.
                    // We don't have enough for hashing, so buffer for next call.
                    data.Slice(transformLength, remainder).CopyTo(_buffer);
                    _bufferPosition = remainder;
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

        /// <summary>
        /// Flushes any buffered data and returns the final hash result.
        /// </summary>
        /// <returns>Hash bytes</returns>
        public Memory<byte> FinalizeHash()
        {
            if(_bufferPosition > 0)
                _algorithm.TransformBlock(_buffer.Slice(0, _bufferPosition));

            _bufferPosition = 0;

            return _algorithm.FinalizeHash();
        }
    }
}
