using System;
using System.Collections.Generic;
using System.Text;

namespace DtronixHash.MurMur3
{
    /// <inheritdoc />
    public abstract class MurMur3Hash128 : NcHashAlgorithm
    {
        /// <summary>
        /// Seed of this instance.
        /// </summary>
        public uint Seed { get; }

        /// <summary>
        /// Length of the read data.
        /// </summary>
        public int Length { get; private set; }

        /// <inheritdoc />
        public override int HashSize => 128;

        /// <inheritdoc />
        public override int InputBlockSize => 128;

        /// <inheritdoc />
        public override int OutputBlockSize => 128;

        /// <inheritdoc />
        protected MurMur3Hash128(uint seed)
        {
            Seed = seed;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            Length = 0;
        }

        /// <inheritdoc />
        public override void TransformBlock(ReadOnlyMemory<byte> memory)
        {
            // store the length of the hash (for use later)
            Length += memory.Length;
        }

        /// <inheritdoc />
        public override Memory<byte> ComputeHash(Memory<byte> block)
        {
            Initialize();
            TransformBlock(block);
            return FinalizeHash();
        }
    }
}
