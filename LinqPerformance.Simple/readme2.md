## Linq List vs Enumerable Performance Comparison

## Benchmark2 Results
- 比較 where 函数在 List 和 Enumerable 上的性能差異
- 比較 where 拆開與合併的性能差異

|                  Method | Size |     Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------------ |----- |---------:|----------:|----------:|-------:|----------:|
|       ListSeparateWhere | 1000 | 5.544 us | 0.2140 us | 0.6209 us | 0.0992 |     648 B |
|        ListCombineWhere | 1000 | 1.441 us | 0.0199 us | 0.0176 us | 0.0229 |     144 B |
| EnumerableSeparateWhere | 1000 | 4.916 us | 0.0218 us | 0.0204 us | 0.0992 |     624 B |
|  EnumerableCombineWhere | 1000 | 1.421 us | 0.0283 us | 0.0532 us | 0.0267 |     168 B |

> Where 拆開, 即使是在 Enumerable 上, 他也不會將其合併! \
> Bill: 若情境複雜, 理想上是抽出 Expression, 在外面處理好, 一次送到 where 裡面去!

### 另外在 LINQPad 看 IL Code 的結果
```csharp
int Size = 100;
void Main()
{
	Method1();	
	Method2();
}

private void Method1()
{
	IEnumerable<int> _enumerable1 = Enumerable.Range(1, Size);
	var result1 = _enumerable1
		.Where(x => x % 2 == 0)
		.Where(x => x % 3 == 0)
		.Where(x => x % 5 == 0)
		.Where(x => x % 7 == 0).ToList();
}

private void Method2()
{
	IEnumerable<int> _enumerable2 = Enumerable.Range(1, Size);
	var result2 = _enumerable2
		.Where(x => x % 2 == 0
			&& x % 3 == 0
			&& x % 5 == 0
			&& x % 7 == 0).ToList();
}
```
![img.png](img.png)

> 可以看到 method1 會產生多個 `Where` 的 IL Code, 而 method2 則是只有一個 `Where` 的 IL Code

## Benchmark3 Results
- 測試 Linq 延遲查詢的性能

|                  Method | Size |     Mean |     Error |    StdDev |   Median |   Gen0 |   Gen1 | Allocated |
|------------------------ |----- |---------:|----------:|----------:|---------:|-------:|-------:|----------:|
|       ListSeparateWhere | 1000 | 5.553 us | 0.1071 us | 0.0949 us | 5.550 us | 0.7553 | 0.0153 |    4744 B |
|        ListCombineWhere | 1000 | 1.563 us | 0.0313 us | 0.0587 us | 1.535 us | 0.6752 | 0.0191 |    4240 B |
| EnumerableSeparateWhere | 1000 | 5.773 us | 0.0491 us | 0.0459 us | 5.790 us | 0.0992 |      - |     624 B |
|  EnumerableCombineWhere | 1000 | 1.558 us | 0.0127 us | 0.0119 us | 1.556 us | 0.0267 |      - |     168 B |

> 有效的省略空間, 但時間上差異不大