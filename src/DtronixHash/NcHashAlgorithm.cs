using System;

namespace DtronixHash.MurMur3
{
    /// <summary>
    /// Non cryptographic hash function interface
    /// </summary>
    public abstract class NcHashAlgorithm
    {
        public abstract int HashSize { get; }
        public abstract int InputBlockSize { get; }
        public abstract int OutputBlockSize { get; }
        public abstract void Initialize();
        public abstract void TransformBlock(ReadOnlyMemory<byte> block);
        public abstract Memory<byte> FinalizeHash();
        public abstract Memory<byte> ComputeHash(Memory<byte> block);
    }
}