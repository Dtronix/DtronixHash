![Icon](src/DtronixHash/icon.png) DtronixHash [![Build Status](https://github.com/Dtronix/DtronixHash/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Dtronix/DtronixHash/actions/workflows/dotnet.yml) [![NuGet](https://img.shields.io/nuget/v/DtronixHash.svg?maxAge=60)](https://www.nuget.org/packages/DtronixHash)
============
DtronixHash contains hashing algorithms utilizing modern .NET methodologies for simple and efficient usage.



#### Hashing Algorithms

| Algorithm   | Cryptographic | References                                                   |
| ----------- | ------------- | ------------------------------------------------------------ |
| MurMur3 x86 | No            | [Implementation Based On](https://github.com/darrenkopp/murmurhash-net) \| [Algorithm Creator](https://en.wikipedia.org/wiki/MurmurHash) |
| MurMur3 x64 | No            | [Implementation Based On](https://github.com/darrenkopp/murmurhash-net) \| [Algorithm Creator](https://en.wikipedia.org/wiki/MurmurHash) |



#### Usage

```c#
var data = new Memory<byte>(new byte[1024 * 1024]);
RandomNumberGenerator.Fill(data.Span);

var algorithm = new MurMur3Hash128X64();
algorithm.TransformBlock(data);

var hashValue = algorithm.FinalizeHash();
```



#### [Benchmarks](docs/Benchmarks/DtronixHash.Benchmarks.MurMur3Benchmark.md)

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19044
Intel Core i7-10700 CPU 2.90GHz, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=6.0.201
  [Host]     : .NET Core 6.0.3 (CoreCLR 6.0.322.12309, CoreFX 6.0.322.12309), X64 RyuJIT
  DefaultJob : .NET Core 6.0.3 (CoreCLR 6.0.322.12309, CoreFX 6.0.322.12309), X64 RyuJIT
```
| Method                       | DataSize      | MBps        | Gen 0      | Gen 1 | Gen 2 | Allocated |
| ---------------------------- | ------------- | ----------- | ----------:| -----:| -----:| ---------:|
| **ComputeMurMur3Hash128X64** | **1000**      | **3,793.9** | **0.0048** | **-** | **-** | **40 B**  |
| ComputeMurMur3Hash128X86     | 1000          | 2,801.7     | 0.0048     | -     | -     | 40 B      |
| ComputeMd5                   | 1000          | 573.7       | 0.0095     | -     | -     | 80 B      |
| ComputeSha256                | 1000          | 252.7       | 0.0114     | -     | -     | 112 B     |
| ComputeSha1                  | 1000          | 611.4       | 0.0114     | -     | -     | 96 B      |
| **ComputeMurMur3Hash128X64** | **100000**    | **5,665.4** | **-**      | **-** | **-** | **40 B**  |
| ComputeMurMur3Hash128X86     | 100000        | 3,717.0     | -          | -     | -     | 40 B      |
| ComputeMd5                   | 100000        | 637.4       | -          | -     | -     | 80 B      |
| ComputeSha256                | 100000        | 274.8       | -          | -     | -     | 112 B     |
| ComputeSha1                  | 100000        | 710.5       | -          | -     | -     | 96 B      |
| **ComputeMurMur3Hash128X64** | **100000000** | **5,054.7** | **-**      | **-** | **-** | **56 B**  |
| ComputeMurMur3Hash128X86     | 100000000     | 3,455.2     | -          | -     | -     | 56 B      |
| ComputeMd5                   | 100000000     | 630.7       | -          | -     | -     | 400 B     |
| ComputeSha256                | 100000000     | 274.3       | -          | -     | -     | 2256 B    |
| ComputeSha1                  | 100000000     | 701.2       | -          | -     | -     | 216 B     |

#### License

MIT