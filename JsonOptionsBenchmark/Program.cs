using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using JsonOptionsBenchmark;

// DefaultConfig 已內建 MarkdownExporter / HtmlExporter / CsvExporter，
// 跑完會在 BenchmarkDotNet.Artifacts/results/ 產出三份報告。
var config = DefaultConfig.Instance.WithOptions(ConfigOptions.JoinSummary);

BenchmarkRunner.Run(
    new[]
    {
        BenchmarkConverter.TypeToBenchmarks(typeof(PureJsonBenchmark), config),
        BenchmarkConverter.TypeToBenchmarks(typeof(EfConverterBenchmark), config),
    });
