using Microsoft.EntityFrameworkCore;

namespace EfVsDapper;

public class MoviesContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    
    protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
    {
        var moviesDbPath = Path.Combine (@"D:\DevSite\DEMO\Benchmarks\EfVsDapper\movies.db");
        optionsBuilder.UseSqlite($"Data Source={moviesDbPath}");
    }
}