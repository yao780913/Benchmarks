# Making Entity Framework Core As Fast As Dapper
[Making Entity Framework Core As Fast As Dapper](https://www.youtube.com/watch?v=OxqAUIYemMs)

``` ini
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.1413)
12th Gen Intel Core i7-12700, 1 CPU, 20 logical and 12 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2  [AttachedDebugger]
  DefaultJob : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2
```

|                 Method |      Mean |     Error |    StdDev |
|----------------------- |----------:|----------:|----------:|
|              EF_Single | 20.062 μs | 0.3789 μs | 0.9848 μs |
|               EF_First | 19.728 μs | 0.3945 μs | 0.8743 μs |
|         Dapper_GetById |  5.070 μs | 0.0543 μs | 0.0508 μs |
| EF_Single_CompileQuery | 12.187 μs | 0.2419 μs | 0.5841 μs |
|  EF_First_CompileQuery | 12.196 μs | 0.2383 μs | 0.3013 μs |
