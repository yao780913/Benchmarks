using Microsoft.EntityFrameworkCore;

namespace EfVsDapper;

public class MoviesContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    
    protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
    {
        var moviesDbPath = Path.Combine (Environment.CurrentDirectory, "movies.db");
        optionsBuilder.UseSqlite($"Data Source={moviesDbPath}");
    }
}