using System.Data;
using Bogus;
using Dapper;

namespace EfVsDapper;

public class MovieGenerator
{
    private readonly IDbConnection _dbConnection;
    private readonly List<Guid> ids = new ();

    private readonly Faker<Movie> _movieGenerator = new Faker<Movie>()
        .RuleFor(m => m.Id, f => f.Random.Guid())
        .RuleFor(m => m.Title, f => f.Name.FullName())
        .RuleFor(m => m.YearOfRelease, f => f.Random.Int(1990, 2023));


    public MovieGenerator (IDbConnection dbConnection, Random random)
    {
        Randomizer.Seed = random;
        _dbConnection = dbConnection;
    }

    public async Task GenerateMovies (int count)
    {
        var generateMovies = _movieGenerator.Generate(count);
        foreach (var generateMovie in generateMovies)
        {
            await _dbConnection.ExecuteAsync(@"
INSERT INTO Movies(Id, Title, YearOfRelease)
VALUES (@Id, @Title, @YearOfRelease)", generateMovie
            );
        }
    }

    public async Task CleanupMovies ()
    {
        await _dbConnection.ExecuteAsync("DELETE FROM Movies");
    }
}