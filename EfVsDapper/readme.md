``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.1413)
12th Gen Intel Core i7-12700, 1 CPU, 20 logical and 12 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2


```
|         Method |        Mean |     Error |    StdDev | Allocated |
|--------------- |------------:|----------:|----------:|----------:|
|        EF_Find |    142.9 ns |   2.86 ns |   3.29 ns |     192 B |
|      EF_Single | 19,762.1 ns | 393.00 ns | 698.55 ns |   10488 B |
|       EF_First | 19,364.5 ns | 385.54 ns | 644.15 ns |   10488 B |
| Dapper_GetById |  5,113.2 ns |  42.15 ns |  35.19 ns |    1992 B |
