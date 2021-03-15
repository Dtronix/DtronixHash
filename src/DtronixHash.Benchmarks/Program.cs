using System;
using System.IO;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using DtronixHash.MurMur3;

namespace DtronixHash.Benchmarks
{
    [MemoryDiagnoser]
    [RPlotExporter]
    public class MurMur3Benchmark
    {
        private byte[] _data;
        private ReadOnlyMemory<byte> _dataMemory;

        private readonly HashAlgorithm _csha1 = SHA1.Create();
        private readonly HashAlgorithm _csha256 = SHA256.Create();
        private readonly MD5 _cmd5 = MD5.Create();
        private readonly NcHashAlgorithm _cmurMur3Hash128X86 = new MurMur3Hash128X86();
        private readonly NcHashAlgorithm _cmurMur3Hash128X64 = new MurMur3Hash128X64();

        private readonly HashAlgorithm _bsha1 = SHA1.Create();
        private readonly HashAlgorithm _bsha256 = SHA256.Create();
        private readonly MD5 _bmd5 = MD5.Create();
        private readonly NcHashAlgorithm _bmurMur3Hash128X86 = new MurMur3Hash128X86();
        private readonly NcHashAlgorithm _bmurMur3Hash128X64 = new MurMur3Hash128X64();

        [Params(1_000, 10_000, 50_000_000)]
        public int DataSize;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data = new byte[DataSize]; // executed once per each DataSize value
            _dataMemory = _data;
            new Random(42).NextBytes(_data);
        }

        [Benchmark]
        public Memory<byte> ComputeMurMur3Hash128X64() => _cmurMur3Hash128X64.ComputeHash(_data);

        [Benchmark]
        public Memory<byte> ComputeMurMur3Hash128X86() => _cmurMur3Hash128X86.ComputeHash(_data);

        [Benchmark]
        public byte[] ComputeMd5() => _cmd5.ComputeHash(_data);

        [Benchmark]
        public byte[] ComputeSha256() => _csha256.ComputeHash(_data);

        [Benchmark]
        public byte[] ComputeSha1() => _csha1.ComputeHash(_data);

        [Benchmark]
        public Memory<byte> BufferMurMur3Hash128X64() => NcHashBufferData(_bmurMur3Hash128X64);

        [Benchmark]
        public Memory<byte> BufferMurMur3Hash128X86() => NcHashBufferData(_bmurMur3Hash128X86);

        [Benchmark]
        public byte[] BufferMd5() => HashData(_bmd5);

        [Benchmark]
        public byte[] BufferSha256() => HashData(_bsha256);

        [Benchmark]
        public byte[] BufferSha1() => HashData(_bsha1);

        public Memory<byte> NcHashBufferData(NcHashAlgorithm algorithm)
        {
            var buffer = new NcHashBuffer(algorithm);
            var iterations = DataSize / 100;
            for (int i = 0; i < iterations; i++)
            {
                buffer.Write(_dataMemory.Slice(i * 100, 100));
            }

            return buffer.FinalizeHash();
        }

        private byte[] HashData(HashAlgorithm algorithm)
        {
            using var inputStream = new MemoryStream(_data);
            return algorithm.ComputeHash(inputStream);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        { 
            BenchmarkRunner.Run<MurMur3Benchmark>();
        }
    }
}
