// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using EfVsDapper;

BenchmarkRunner.Run<Benchmarks>();