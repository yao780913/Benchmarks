using BenchmarkDotNet.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JsonOptionsBenchmark;

/// <summary>
/// Group 2 — EF Core HasConversion 真實情境。
/// SQLite in-memory，預塞 N 筆 JobApplicationRow（含 JSON 序列化的 CandidateSnapshot），
/// 然後 ToList 讀完全部，比較兩種 converter pattern。
/// </summary>
[MemoryDiagnoser]
[HideColumns("Error", "StdDev")]
public class EfConverterBenchmark
{
    [Params(1, 10, 50, 200, 1000)]
    public int RowCount { get; set; }

    private SqliteConnection _sharedConn = null!;
    private SqliteConnection _freshConn  = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Shared options DB
        _sharedConn = new SqliteConnection("DataSource=:memory:");
        _sharedConn.Open();
        using (var ctx = NewSharedCtx())
        {
            ctx.Database.EnsureCreated();
            for (int i = 0; i < RowCount; i++)
                ctx.Rows.Add(TestData.SampleRow(i));
            ctx.SaveChanges();
        }

        // Fresh-per-call DB
        _freshConn = new SqliteConnection("DataSource=:memory:");
        _freshConn.Open();
        using (var ctx = NewFreshCtx())
        {
            ctx.Database.EnsureCreated();
            for (int i = 0; i < RowCount; i++)
                ctx.Rows.Add(TestData.SampleRow(i));
            ctx.SaveChanges();
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _sharedConn.Dispose();
        _freshConn.Dispose();
    }

    [Benchmark(Baseline = true)]
    public List<JobApplicationRow> Ef_Shared()
    {
        using var ctx = NewSharedCtx();
        return ctx.Rows.AsNoTracking().ToList();
    }

    [Benchmark]
    public List<JobApplicationRow> Ef_NewPerCall()
    {
        using var ctx = NewFreshCtx();
        return ctx.Rows.AsNoTracking().ToList();
    }

    private SharedDbContext NewSharedCtx() => new(
        new DbContextOptionsBuilder<SharedDbContext>()
            .UseSqlite(_sharedConn)
            .Options);

    private FreshDbContext NewFreshCtx() => new(
        new DbContextOptionsBuilder<FreshDbContext>()
            .UseSqlite(_freshConn)
            .Options);
}
