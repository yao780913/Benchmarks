## Linq List vs Enumerable Performance Comparison


|                  Method | Size |     Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------------ |----- |---------:|----------:|----------:|-------:|----------:|
|       ListSeparateWhere | 1000 | 5.544 us | 0.2140 us | 0.6209 us | 0.0992 |     648 B |
|        ListCombineWhere | 1000 | 1.441 us | 0.0199 us | 0.0176 us | 0.0229 |     144 B |
| EnumerableSeparateWhere | 1000 | 4.916 us | 0.0218 us | 0.0204 us | 0.0992 |     624 B |
|  EnumerableCombineWhere | 1000 | 1.421 us | 0.0283 us | 0.0532 us | 0.0267 |     168 B |
