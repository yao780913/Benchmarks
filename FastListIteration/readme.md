### Benchmark

```ini
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.2006/21H1/May2021Update)
Intel Core i5-6500 CPU 3.20GHz (Skylake), 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.400
 [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2  [AttachedDebugger]
 DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2

```  

| Method           |  Size |         Mean |      Error |       StdDev |       Median | Allocated |
|------------------|------ |-------------:|-----------:|-------------:|-------------:|----------:|
|                  |   100 |
| For              |   100 |     69.87 ns |   1.104 ns |     0.922 ns |     69.78 ns |         - |
| Foreach          |   100 |    137.22 ns |   2.782 ns |     4.648 ns |    136.10 ns |         - |
| Foreach_Linq     |   100 |    230.20 ns |   2.178 ns |     1.931 ns |    230.37 ns |         - |
| Parallel_Foreach |   100 |  2,503.89 ns |  15.583 ns |    14.577 ns |  2,501.47 ns |    1856 B |
| Parallel_Linq    |   100 |  3,067.15 ns |  31.407 ns |    27.842 ns |  3,072.17 ns |    2840 B |
| ForEach_Span     |   100 |     44.22 ns |   0.837 ns |     0.783 ns |     44.16 ns |         - |
| For_Span         |   100 |     78.53 ns |   0.614 ns |     0.544 ns |     78.35 ns |         - |
|                  |  1000 |
| For              |  1000 |    670.12 ns |  12.527 ns |    11.718 ns |    670.76 ns |         - |
| Foreach          |  1000 |  1,325.48 ns |  26.428 ns |    34.364 ns |  1,324.95 ns |         - |
| Foreach_Linq     |  1000 |  2,354.91 ns |  46.008 ns |    67.438 ns |  2,334.16 ns |         - |
| Parallel_Foreach |  1000 |  6,719.98 ns | 131.707 ns |   140.926 ns |  6,697.78 ns |    2107 B |
| Parallel_Linq    |  1000 |  7,267.39 ns | 168.443 ns |   494.015 ns |  7,221.59 ns |    2840 B |
| ForEach_Span     |  1000 |    369.26 ns |   7.251 ns |     7.759 ns |    367.89 ns |         - |
| For_Span         |  1000 |    674.99 ns |  10.501 ns |     9.823 ns |    672.56 ns |         - |
|                  | 10000 |
| For              | 10000 |  6,398.86 ns | 120.824 ns |   113.019 ns |  6,359.55 ns |         - |
| Foreach          | 10000 | 12,408.67 ns | 136.452 ns |   127.637 ns | 12,358.40 ns |         - |
| Foreach_Linq     | 10000 | 22,373.08 ns | 358.961 ns |   318.210 ns | 22,381.46 ns |         - |
| Parallel_Foreach | 10000 | 23,001.54 ns | 446.149 ns |   495.894 ns | 23,004.84 ns |    2227 B |
| Parallel_Linq    | 10000 | 23,420.21 ns | 465.313 ns | 1,312.422 ns | 23,014.18 ns |    2840 B |
| ForEach_Span     | 10000 |  3,835.96 ns |  75.448 ns |   112.927 ns |  3,821.71 ns |         - |
| For_Span         | 10000 |  6,979.33 ns | 136.748 ns |   162.789 ns |  6,972.53 ns |         - |

### Summary
* `For` 優於 `Foreach`
* `Span` 效能最佳
* 不要使用 `Foreach_Linq` (EX: `items.Foreach(item => { })`)

### References
[連結](https://www.youtube.com/watch?v=jUZ3VKFyB-A&t=321s)