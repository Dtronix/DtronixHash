using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using DtronixHash.MurMur3;
using NUnit.Framework;

namespace DtronixHash.Tests
{
    public class Tests
    {
        public enum HashAlgorithms
        {
            MurMur3Hash128X64,
            MurMur3Hash128X86
        }

        private static readonly object[][] HashAlgorithms_consistentTestCases;

        static Tests()
        {
            var cases = new List<object[]>();
            var algorithms = (HashAlgorithms[]) Enum.GetValues(typeof(HashAlgorithms));

            foreach (var algorithm in algorithms)
            {
                cases.Add(new object[] {algorithm, 1});
                cases.Add(new object[] {algorithm, 128});
                cases.Add(new object[] {algorithm, 1024 * 8});
                cases.Add(new object[] {algorithm, 1000 * 5}); // Non-multiple
            }

            HashAlgorithms_consistentTestCases = cases.ToArray();
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCaseSource(nameof(HashAlgorithms_consistentTestCases))]
        public void HashAlgorithms_consistent(HashAlgorithms algorithm, int size)
        {
            var bytes = new byte[size];
            RandomNumberGenerator.Fill(bytes);
            var value1 = GetHashValue(GetHashAlgorithm(algorithm), bytes);
            var value2 = GetHashValue(GetHashAlgorithm(algorithm), bytes);
            Assert.AreEqual(value1.ToArray(), value2.ToArray());
        }

        [Test]
        [TestCase(HashAlgorithms.MurMur3Hash128X64)]
        [TestCase(HashAlgorithms.MurMur3Hash128X86)]
        public void HashAlgorithms_hashes_multiple_of_size(HashAlgorithms algorithm)
        {
            var sourceHashAlgorithm = GetHashAlgorithm(algorithm);
            var bytes = new byte[sourceHashAlgorithm.InputBlockSize * 5];
            RandomNumberGenerator.Fill(bytes);
            var value1 = GetHashValue(sourceHashAlgorithm, bytes);

            var hash = GetHashAlgorithm(algorithm);

            var mem = new Memory<byte>(bytes);

            for (int i = 0; i < 5; i++)
                hash.TransformBlock(mem.Slice(i * sourceHashAlgorithm.InputBlockSize, sourceHashAlgorithm.InputBlockSize));

            Assert.AreEqual(value1.ToArray(), hash.FinalizeHash().ToArray());
        }

        [Test]
        [TestCase(HashAlgorithms.MurMur3Hash128X64)]
        [TestCase(HashAlgorithms.MurMur3Hash128X86)]
        public void NcHashBuffer_hashes_small_buffers(HashAlgorithms algorithm)
        {
            var sourceHashAlgorithm = GetHashAlgorithm(algorithm);
            var loops = 100;
            var bytes = new byte[7 * loops];
            
            RandomNumberGenerator.Fill(bytes);
            var value1 = GetHashValue(sourceHashAlgorithm, bytes);

            var hash = GetHashAlgorithm(algorithm);

            var mem = new Memory<byte>(bytes);

            var buffer = new NcHashBuffer(hash);

            for (int i = 0; i < loops; i++)
            {
                buffer.Write(mem.Slice(i * 7, 7));
            }

            Assert.AreEqual(value1.ToArray(), buffer.FinalizeHash().ToArray());
        }

        [Test]
        [TestCase(HashAlgorithms.MurMur3Hash128X64)]
        [TestCase(HashAlgorithms.MurMur3Hash128X86)]
        public void NcHashBuffer_hashes_medium_buffers(HashAlgorithms algorithm)
        {
            var sourceHashAlgorithm = GetHashAlgorithm(algorithm);
            var loops = 10;
            var bytes = new byte[1000];

            RandomNumberGenerator.Fill(bytes);

            var correctHashValue = GetHashValue(sourceHashAlgorithm, bytes);

            var hash = GetHashAlgorithm(algorithm);
            var mem = new Memory<byte>(bytes);
            var buffer = new NcHashBuffer(hash);

            for (int i = 0; i < loops; i++)
            {
                buffer.Write(mem.Slice(i * (bytes.Length/loops), bytes.Length / loops));
            }

            var calculatedHashValue = buffer.FinalizeHash();

            Assert.AreEqual(correctHashValue.ToArray(), calculatedHashValue.ToArray());
        }

        private Memory<byte> GetHashValue(NcHashAlgorithm algorithm, ReadOnlyMemory<byte> data)
        {
            algorithm.TransformBlock(data);
            return algorithm.FinalizeHash();
        }

        private NcHashAlgorithm GetHashAlgorithm(HashAlgorithms algorithm)
        {
            return algorithm switch
            {
                HashAlgorithms.MurMur3Hash128X64 => new MurMur3Hash128X64(),
                HashAlgorithms.MurMur3Hash128X86 => new MurMur3Hash128X86(),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null)
            };
        }
    }
}