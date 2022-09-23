``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19044.1889/21H2/November2021Update)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.304
  [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2  [AttachedDebugger]
  DefaultJob : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2


```
|           Method |  Size |          Mean |        Error |       StdDev |        Median | Allocated |
|----------------- |------ |--------------:|-------------:|-------------:|--------------:|----------:|
|              **For** |   **100** |      **66.05 ns** |     **4.924 ns** |     **13.89 ns** |      **61.32 ns** |         **-** |
|          Foreach |   100 |      93.98 ns |     3.725 ns |     10.32 ns |      91.89 ns |         - |
|     Foreach_Linq |   100 |     271.53 ns |    26.058 ns |     76.83 ns |     243.00 ns |         - |
| Parallel_Foreach |   100 |  20,181.45 ns |   233.115 ns |    206.65 ns |  20,170.72 ns |    2426 B |
|    Parallel_Linq |   100 |  30,584.40 ns |   440.929 ns |    412.44 ns |  30,485.84 ns |    4088 B |
|     ForEach_Span |   100 |      55.80 ns |     3.926 ns |     10.94 ns |      51.95 ns |         - |
|         For_Span |   100 |      81.92 ns |     7.200 ns |     19.22 ns |      72.97 ns |         - |
|              **For** |  **1000** |     **633.88 ns** |    **82.062 ns** |    **228.76 ns** |     **532.19 ns** |         **-** |
|          Foreach |  1000 |     942.44 ns |    79.587 ns |    233.41 ns |     841.53 ns |         - |
|     Foreach_Linq |  1000 |   3,190.05 ns |   521.696 ns |  1,401.50 ns |   2,647.51 ns |         - |
| Parallel_Foreach |  1000 |  40,625.41 ns |   508.951 ns |    451.17 ns |  40,583.85 ns |    2953 B |
|    Parallel_Linq |  1000 |  37,026.11 ns |   739.924 ns |    962.11 ns |  36,970.99 ns |    4088 B |
|     ForEach_Span |  1000 |     628.11 ns |    55.052 ns |    161.46 ns |     576.92 ns |         - |
|         For_Span |  1000 |     737.02 ns |    52.574 ns |    147.42 ns |     689.39 ns |         - |
|              **For** | **10000** |   **6,905.98 ns** |   **761.814 ns** |  **2,185.79 ns** |   **6,632.36 ns** |         **-** |
|          Foreach | 10000 |   9,779.35 ns |   914.534 ns |  2,579.46 ns |   9,085.33 ns |         - |
|     Foreach_Linq | 10000 |  29,341.95 ns | 3,191.042 ns |  9,155.70 ns |  25,467.04 ns |         - |
| Parallel_Foreach | 10000 | 116,608.92 ns | 2,295.484 ns |  3,640.88 ns | 115,428.12 ns |    3022 B |
|    Parallel_Linq | 10000 | 147,192.56 ns | 4,956.341 ns | 14,613.88 ns | 147,736.73 ns |    4089 B |
|     ForEach_Span | 10000 |   5,877.87 ns |   450.864 ns |  1,147.59 ns |   5,304.71 ns |         - |
|         For_Span | 10000 |  24,704.96 ns | 4,270.360 ns | 12,591.25 ns |  25,476.75 ns |         - |

### Summary
* `For` 優於 `Foreach`
* 不要用 `items.Foreach(item => { ... })`
* `Span` 效能最佳

### References
[The fastest way to iterate a List in C# is NOT what you think](https://www.youtube.com/watch?v=jUZ3VKFyB-A)