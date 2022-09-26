using BenchmarkDotNet.Attributes;

namespace EmptyEnumerable;

[MemoryDiagnoser(false)]
public class Benchmarks
{
    private class Foo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Benchmark]
    public void Empty () => Enumerable.Empty<Foo>();

    [Benchmark]
    public void NewList () => new List<Foo>();
}