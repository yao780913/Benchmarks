using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JsonOptionsBenchmark;

/// <summary>
/// HasConversion 使用 <see cref="JsonOptionsFactory.Shared"/>（共享單例）。
/// </summary>
public sealed class SharedDbContext : DbContext
{
    public DbSet<JobApplicationRow> Rows => Set<JobApplicationRow>();

    public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<JobApplicationRow>(e =>
        {
            e.ToTable("Rows");
            e.HasKey(x => x.Id);
            e.Property(x => x.ApplicantName).HasMaxLength(100);
            e.Property(x => x.Candidate)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptionsFactory.Shared),
                    v => JsonSerializer.Deserialize<CandidateSnapshot>(v, JsonOptionsFactory.Shared)!);
        });
    }
}

/// <summary>
/// HasConversion 每次都 <see cref="JsonOptionsFactory.FreshPerCall"/>（壞 pattern）。
/// </summary>
public sealed class FreshDbContext : DbContext
{
    public DbSet<JobApplicationRow> Rows => Set<JobApplicationRow>();

    public FreshDbContext(DbContextOptions<FreshDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<JobApplicationRow>(e =>
        {
            e.ToTable("Rows");
            e.HasKey(x => x.Id);
            e.Property(x => x.ApplicantName).HasMaxLength(100);
            e.Property(x => x.Candidate)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptionsFactory.FreshPerCall()),
                    v => JsonSerializer.Deserialize<CandidateSnapshot>(v, JsonOptionsFactory.FreshPerCall())!);
        });
    }
}
