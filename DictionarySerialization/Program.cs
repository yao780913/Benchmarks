using BenchmarkDotNet.Running;
using DictionarySerialization;

var summary = BenchmarkRunner.Run<DictionaryDeserializationBenchmark>();