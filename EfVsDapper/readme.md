## [Entity Framework Core vs Dapper Performance in 2023](https://www.youtube.com/watch?v=Q4LtKa_HTHU)
Nick Chapsas - YT - Entity Framework Core vs Dapper Performance in 2023

## EF_Find , EF_Single , EF_First and Dapper_GetById
``` ini
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.1413)
12th Gen Intel Core i7-12700, 1 CPU, 20 logical and 12 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2
```
|         Method |        Mean |     Error |    StdDev | Allocated |
|--------------- |------------:|----------:|----------:|----------:|
|    ~~EF_Find~~ |    142.9 ns |   2.86 ns |   3.29 ns |     192 B |
|      EF_Single | 19,762.1 ns | 393.00 ns | 698.55 ns |   10488 B |
|       EF_First | 19,364.5 ns | 385.54 ns | 644.15 ns |   10488 B |
| Dapper_GetById |  5,113.2 ns |  42.15 ns |  35.19 ns |    1992 B |

* **EF_Find 的結果並不準確**, 實際上在第一次執行時, 就已經將結果存進 memory 了, 之後的動作都只是跟 memory 拿資料, 並沒有真的 attach 到 db
* 加上 `NoTracking()` 確實可以讓結果在更快一些, 不過這不在這次討論範圍內, 就此略過


## Add and Delete
```
|            Method |     Mean |    Error |   StdDev | Allocated |
|------------------ |---------:|---------:|---------:|----------:|
|     EF_Add_Delete | 56.17 ms | 3.168 ms | 9.242 ms |  29.22 KB |
| Dapper_Add_Delete | 58.83 ms | 2.631 ms | 7.716 ms |   3.95 KB |
```

## UPDATE
```
|        Method |     Mean |    Error |   StdDev | Allocated |
|-------------- |---------:|---------:|---------:|----------:|
|     EF_Update | 28.04 ms | 1.548 ms | 4.515 ms |  14.36 KB |
| Dapper_Update | 28.66 ms | 1.347 ms | 3.950 ms |      2 KB |
```

## Filtering
```
|        Method |     Mean |    Error |   StdDev | Allocated |
|-------------- |---------:|---------:|---------:|----------:|
|     EF_Filter | 22.73 μs | 0.440 μs | 0.412 μs |  11.36 KB |
| Dapper_Filter | 10.54 μs | 0.046 μs | 0.043 μs |   3.14 KB |
```

### 註
在 Sqlite 內使用 `FirstOrDefaultAsync()` or `SingleOrDefaultAsync()` 會有錯誤

<p style="color: red;">The provider for the source IQueryable doesn't implement IDbAsyncQueryProvider. Only providers that implement IDbAsyncQueryProvider can be used for Entity Framework asynchronous operations.</p>

所以以下測試案例都不使用非同步語法

