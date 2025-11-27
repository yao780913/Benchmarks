# Dictionary 序列化性能測試

## 目的
測試從 Dictionary 中提取物件的不同方式，比較暴力序列化方案與優化方案的性能差異。

## 測試場景
1. **Guid**：簡單型別轉換
2. **SimpleObject**：單個物件
3. **List<string>**：字串列表
4. **List<ComplexObject>**：複雜物件列表
5. **混合場景**：同時提取所有類型

## 核心結論
✅ **直接型別檢查優於暴力序列化**

| 場景 | 性能提升 |
|------|---------|
| Guid | **6.8 倍** ⚡ |
| 混合場景 | **1.84 倍** |
| 內存分配 | **100% 減少** |

## 使用方式
```bash
dotnet run -c Release
```

結果會顯示每個 Benchmark 的詳細性能數據，包括平均時間、標準差和內存使用情況。


## 測試結果
### Raw Data
``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26100.7171)
Unknown processor
.NET SDK=9.0.302
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-AJYMBL : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  


```
|                                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|--------------------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
|              Benchmark_BruteForce_Guid | 67.397 ns | 33.062 ns | 8.5862 ns |  1.00 |    0.00 | 0.0153 |      96 B |        1.00 |
|               Benchmark_Optimized_Guid |  9.902 ns |  4.962 ns | 1.2887 ns |  0.15 |    0.01 |      - |         - |        0.00 |
|      Benchmark_BruteForce_SimpleObject | 13.697 ns |  4.862 ns | 1.2626 ns |  0.21 |    0.04 |      - |         - |        0.00 |
|       Benchmark_Optimized_SimpleObject | 10.946 ns |  1.721 ns | 0.4469 ns |  0.16 |    0.02 |      - |         - |        0.00 |
|        Benchmark_BruteForce_ListString | 13.051 ns |  6.474 ns | 1.6813 ns |  0.20 |    0.04 |      - |         - |        0.00 |
|         Benchmark_Optimized_ListString | 11.452 ns |  9.663 ns | 2.5094 ns |  0.17 |    0.05 |      - |         - |        0.00 |
| Benchmark_BruteForce_ListComplexObject | 11.713 ns |  1.594 ns | 0.2467 ns |  0.18 |    0.02 |      - |         - |        0.00 |
|  Benchmark_Optimized_ListComplexObject | 10.334 ns |  1.787 ns | 0.4640 ns |  0.16 |    0.02 |      - |         - |        0.00 |
|      Benchmark_BruteForce_AllScenarios | 72.129 ns | 17.478 ns | 2.7047 ns |  1.12 |    0.14 | 0.0153 |      96 B |        1.00 |
|       Benchmark_Optimized_AllScenarios | 39.250 ns |  2.713 ns | 0.4198 ns |  0.61 |    0.07 |      - |         - |        0.00 |


### AI: 關鍵洞察

1. Guid 轉換最受益 ✨
    - 暴力方式：67.397 ns（需要序列化/反序列化）
    - 優化方式：9.902 ns（直接型別檢查）
    - 節省 57.5 ns，性能提升 6.8 倍
2. 內存分配大幅減少
    - 暴力方式：每次操作分配 96B
    - 優化方式：零分配 ✅
    - 特別是在高頻操作中，這意味著更少的 GC 壓力
3. Object 和 List 場景
    - 性能差異相對較小（10-25%）
    - 因為當對象已經在記憶體中時，型別檢查已經很快了
    - 暴力方法的開銷被攤平
4. 混合場景
    - 所有操作執行時：暴力方式 72.129 ns vs 優化方式 39.250 ns
    - 節省 32.879 ns，性能提升 1.84 倍