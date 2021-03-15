using System;

namespace DtronixHash
{
    /// <summary>
    /// Non cryptographic hash function interface
    /// </summary>
    public abstract class NcHashAlgorithm
    {
        /// <summary>
        /// Size of the hash in bits.
        /// </summary>
        public abstract int HashSize { get; }

        /// <summary>
        /// Size of the full input blocks.
        /// </summary>
        public abstract int InputBlockSize { get; }

        /// <summary>
        /// Size of the hash result.
        /// </summary>
        public abstract int OutputBlockSize { get; }

        /// <summary>
        /// Resets the hash to its initial state.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Transforms the passed block of data into the hash.
        /// </summary>
        /// <param name="block">Data to hash.</param>
        public abstract void TransformBlock(ReadOnlyMemory<byte> block);

        /// <summary>
        /// Completes the input transformation and returns the block's hash.
        /// </summary>
        /// <returns>Hash of data.</returns>
        public abstract Memory<byte> FinalizeHash();

        /// <summary>
        /// Hashes the entire passed block and finalizes the output.
        /// </summary>
        /// <param name="block">Complete data to hash.</param>
        /// <returns>Hash of passed data.</returns>
        public abstract Memory<byte> ComputeHash(Memory<byte> block);
    }
}