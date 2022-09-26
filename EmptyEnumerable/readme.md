``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.2006/21H1/May2021Update)
Intel Core i5-6500 CPU 3.20GHz (Skylake), 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.400
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2  [AttachedDebugger]
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2

```
|  Method |      Mean |     Error |    StdDev |    Median | Allocated |
|-------- |----------:|----------:|----------:|----------:|----------:|
|   Empty | 0.0024 ns | 0.0042 ns | 0.0043 ns | 0.0000 ns |         - |
| NewList | 7.8364 ns | 0.1907 ns | 0.4570 ns | 7.6762 ns |      32 B |

### Summary
* 不回傳 Null
* 使用 `Emunerable.Empty<T>()` 回傳空陣列