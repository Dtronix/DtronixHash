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

        private readonly HashAlgorithm _sha1 = SHA1.Create();
        private readonly HashAlgorithm _sha256 = SHA256.Create();
        private readonly MD5 _md5 = MD5.Create();
        private readonly NcHashAlgorithm _murMur3Hash128X86 = new MurMur3Hash128X86();
        private readonly NcHashAlgorithm _murMur3Hash128X64 = new MurMur3Hash128X64();

        [Params(1_000, 10_000, 50_000_000)]
        public int DataSize;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data = new byte[DataSize]; // executed once per each DataSize value
            new Random(42).NextBytes(_data);
        }

        [Benchmark]
        public Memory<byte> MurMur3Hash128X64()
        {
            return _murMur3Hash128X64.ComputeHash(_data);
        }

        [Benchmark]
        public Memory<byte> MurMur3Hash128X86()
        {
            return _murMur3Hash128X86.ComputeHash(_data);
        }

        [Benchmark]
        public byte[] Md5() => _md5.ComputeHash(_data);

        [Benchmark]
        public byte[] Sha256() => _sha256.ComputeHash(_data);

        [Benchmark]
        public byte[] Sha1() => _sha1.ComputeHash(_data);
    }

    [MemoryDiagnoser]
    [RPlotExporter]
    public class NcHashBufferBenchmark
    {
        private byte[] _data;

        private ReadOnlyMemory<byte> _dataMemory;

        private readonly HashAlgorithm _sha1 = SHA1.Create();
        private readonly HashAlgorithm _sha256 = SHA256.Create();
        private readonly MD5 _md5 = MD5.Create();
        private readonly NcHashAlgorithm _murMur3Hash128X86 = new MurMur3Hash128X86();
        private readonly NcHashAlgorithm _murMur3Hash128X64 = new MurMur3Hash128X64();

        [Params(1_000, 10_000, 50_000_000)]
        public int DataSize;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data = new byte[DataSize]; // executed once per each DataSize value
            _dataMemory = _data; // executed once per each DataSize value
            new Random(42).NextBytes(_data);
        }

        [Benchmark]
        public Memory<byte> MurMur3Hash128X64() => NcHashBufferData(_murMur3Hash128X86);

        [Benchmark]
        public Memory<byte> MurMur3Hash128X86() => NcHashBufferData(_murMur3Hash128X86);

        [Benchmark]
        public byte[] Md5() => HashData(_md5);

        [Benchmark]
        public byte[] Sha256() => HashData(_sha256);

        [Benchmark]
        public byte[] Sha1() => HashData(_sha1);

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

        private byte[] HashData(ICryptoTransform transformer)
        {
            var inputStream = new MemoryStream(_data);
            var cryptoStream = new CryptoStream(inputStream, transformer, CryptoStreamMode.Read);
            var buffer = new Span<byte>(new byte[100]);
            int readLength = 0;
            int totalRead = 0;
            while ((readLength = cryptoStream.Read(buffer)) > 0)
            {
                totalRead += readLength;
                if (totalRead == _data.Length)
                    break;
            }
            cryptoStream.FlushFinalBlock();

            return _sha1.Hash;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<MurMur3Benchmark>();
            BenchmarkRunner.Run<NcHashBufferBenchmark>();
        }
    }
}
