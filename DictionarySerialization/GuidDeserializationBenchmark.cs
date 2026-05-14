using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace DictionarySerialization;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, targetCount: 5)]
public class GuidDeserializationBenchmark
{
    private Dictionary<string, object> _testData = null!;
    private readonly DictionaryDeserializer _deserializer = new();
    private Guid _testGuid = Guid.NewGuid();

    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    [GlobalSetup]
    public void Setup()
    {
        _testData = TestDataGenerator.GenerateTestData();
        _testGuid = (Guid)_testData["propertyA"];
    }

    /// <summary>
    /// Approach 1: Direct casting (as Guid)
    /// </summary>
    [Benchmark]
    public Guid BenchmarkAsGuid()
    {
        return _testData["propertyA"] as Guid? ?? Guid.Empty;
    }

    /// <summary>
    /// Approach 2: Using DeserializeToObject<Guid>
    /// </summary>
    [Benchmark]
    public Guid BenchmarkDeserializeToObject()
    {
        return _deserializer.DeserializeToObject<Guid>(_testData["propertyA"]);
    }

    /// <summary>
    /// Approach 3: Using DeserializeToGuid (specialized)
    /// </summary>
    [Benchmark]
    public Guid BenchmarkDeserializeToGuid()
    {
        return _deserializer.DeserializeToGuid(_testData["propertyA"]);
    }
}
