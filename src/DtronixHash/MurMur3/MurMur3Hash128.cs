using System;
using System.Collections.Generic;
using System.Text;

namespace DtronixHash.MurMur3
{
    internal abstract class MurMur3Hash128 : NcHashAlgorithm
    {
        public uint Seed { get; }
        public int Length { get; private set; }
        public override int HashSize => 128;
        public override int InputBlockSize => 128;
        public override int OutputBlockSize => 128;

        protected MurMur3Hash128(uint seed)
        {
            Seed = seed;
        }

        public override void Initialize()
        {
            Length = 0;
        }

        public override void TransformBlock(ReadOnlyMemory<byte> memory)
        {
            // store the length of the hash (for use later)
            Length += memory.Length;
        }

        public override Memory<byte> ComputeHash(Memory<byte> block)
        {
            Initialize();
            TransformBlock(block);
            return FinalizeHash();
        }
    }
}
