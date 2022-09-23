# Is LINQ in C# actually slow?
<style>
    .red { color: red;}
</style>
> https://www.youtube.com/watch?v=8-NAwKYXMzs

* Avoid allocations in compiler hot paths:
  * Avoid LINQ.
  * Avoid using foreach over collecitons that do not have a struct enumerator.
  * Consider using an object pool. There are many usages of object pools in the compiler to see an example.

|                        Method |     Mean |   Error |   StdDev |   Median |
|------------------------------ |---------:|--------:|---------:|---------:|
| AlcoholicDrinksCountLinqWhere | 447.8 us | 6.85 us | 17.44 us | 442.1 us |
| AlcoholicDrinksCountLinqCount | 876.7 us | 4.47 us |  3.96 us | 876.3 us |


|                         Method |     Mean |    Error |   StdDev |   Median |
|------------------------------- |---------:|---------:|---------:|---------:|
|  AlcoholicDrinksCountLinqWhere | 558.2 us | 10.49 us |  9.30 us | 557.9 us |
|  AlcoholicDrinksCountLinqCount | 995.5 us | 16.63 us | 13.88 us | 997.1 us |
|     AlcoholicDrinkCountForLoop | 525.7 us | 10.38 us | 10.65 us | 524.7 us |
| AlcoholicDrinkCountForeachLoop | 685.0 us | 16.60 us | 46.28 us | 668.9 us |

|                         Method |     Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |---------:|---------:|---------:|------:|------:|------:|----------:|
|  AlcoholicDrinksCountLinqWhere | 452.7 us |  5.64 us |  5.28 us |     - |     - |     - |      72 B |
|  AlcoholicDrinksCountLinqCount | 888.7 us |  4.54 us |  4.25 us |     - |     - |     - |      40 B |
|     AlcoholicDrinkCountForLoop | 420.7 us |  7.58 us |  7.09 us |     - |     - |     - |         - |
| AlcoholicDrinkCountForeachLoop | 642.4 us | 12.39 us | 11.59 us |     - |     - |     - |       1 B |


|                        Method |     Mean |   Error |  StdDev |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------------------------------ |---------:|--------:|--------:|--------:|--------:|--------:|----------:|
| NonAlcoholicDrinkNamesForLoop | 646.2 us | 3.14 us | 2.78 us | 70.3125 | 43.9453 | 42.9688 | 512.64 KB |
|    NonAlcoholicDrinkNamesLinq | 738.3 us | 5.64 us | 5.28 us | 71.2891 | 44.9219 | 43.9453 | 512.76 KB |

> The `Where` and `select` will use a similar `Enumerator` to operate so you  won't actually do it twice and that is reflected in the performance of the code.  
So LINQ will actually optimize for that.  

> But also `ToList()` anything starts with <strong class='red bold'>`To`</strong> when it comes down to extensions.  
For example: `ToArray()`, `ToDiciationary()`...  
**It will actually create the object it's talking about the allocated that memory.**

> Anything that starts with <strong class='red bold'>`As`</strong>  
> `AsEnumerable()`, `AsParallel()`  
> **It just cast it.**