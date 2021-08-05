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



#### Benchmarks
``` ini
Intel Core i7-10700 CPU 2.90GHz, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.200
```
| Method                       | Data (KB)       |        MBps |      Gen 0 | Gen 1 | Gen 2 | Allocated |
| :--------------------------- | --------------- | ----------: | ---------: | ----: | ----: | --------: |
| **ComputeMurMur3Hash128X64** | **1,000**       | **3,609.0** | **0.0048** | **-** | **-** |  **40 B** |
| **ComputeMurMur3Hash128X86** | **1,000**       | **2,666.1** | **0.0048** | **-** | **-** |  **40 B** |
| ComputeMd5                   | 1,000           |       527.9 |     0.0095 |     - |     - |      80 B |
| ComputeSha256                | 1,000           |       255.2 |     0.0114 |     - |     - |     112 B |
| ComputeSha1                  | 1,000           |       620.5 |     0.0114 |     - |     - |      96 B |
| **ComputeMurMur3Hash128X64** | **100,000**     | **5,513.4** |      **-** | **-** | **-** |  **40 B** |
| **ComputeMurMur3Hash128X86** | **100,000**     | **3,639.6** |      **-** | **-** | **-** |  **40 B** |
| ComputeMd5                   | 100,000         |       582.3 |          - |     - |     - |      80 B |
| ComputeSha256                | 100,000         |       277.9 |          - |     - |     - |     112 B |
| ComputeSha1                  | 100,000         |       715.6 |          - |     - |     - |      96 B |
| **ComputeMurMur3Hash128X64** | **100,000,000** | **4,820.6** |      **-** | **-** | **-** |  **40 B** |
| **ComputeMurMur3Hash128X86** | **100,000,000** | **3,320.4** |      **-** | **-** | **-** |  **40 B** |
| ComputeMd5                   | 100,000,000     |       578.9 |          - |     - |     - |     228 B |
| ComputeSha256                | 100,000,000     |       276.3 |          - |     - |     - |     112 B |
| ComputeSha1                  | 100,000,000     |       702.1 |          - |     - |     - |      96 B |

#### License

MIT