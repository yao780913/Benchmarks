using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using EfVsDapper.Database;

namespace EfVsDapper;

[MemoryDiagnoser(false)]
public class Benchmarks
{
    private MoviesContext _moviesContext = null;
    private IDbConnection _dbConnection = null;
    private Random _random = null;
    private Movie _testMovie = null;
    private MovieGenerator _movieGenerator = null;

    [GlobalSetup]
    public async Task Setup ()
    {
        _random = new Random(420);
        var dbConnectionFactory = new SqliteConnectionFactory(@"Data Source=D:\DevSite\DEMO\Benchmarks\EfVsDapper\movies.db");
        _dbConnection = await dbConnectionFactory.CreateConnectionAsync();

        _movieGenerator = new MovieGenerator(_dbConnection, _random);
        await _movieGenerator.GenerateMovies(100);

        _testMovie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Test Movie",
            YearOfRelease = _random.Next()
        };

        await _dbConnection.ExecuteAsync(@"
INSERT INTO Movies (Id, Title, YearOfRelease)
VALUES (@Id, @Title, @YearOfRelease)", _testMovie);

        _moviesContext = new ();
    }

    [GlobalCleanup]
    public async Task Cleanup ()
    {
        await _movieGenerator.CleanupMovies();
        await _dbConnection.ExecuteAsync("DELETE FROM Movies WHERE Id = @Id", _testMovie);
    }
    
    [Benchmark]
    public Movie? EF_Find ()
    {
        return _moviesContext.Movies.Find(_testMovie.Id);
    }
    
    [Benchmark]
    public Movie? EF_Single ()
    {
        return _moviesContext.Movies.SingleOrDefault(x => x.Id == _testMovie.Id);
    }

    [Benchmark]
    public Movie? EF_First ()
    {
        return _moviesContext.Movies.FirstOrDefault(x => x.Id == _testMovie.Id);
    }

    [Benchmark]
    public Movie? Dapper_GetById ()
    {
        return _dbConnection.QuerySingleOrDefault<Movie>("SELECT * FROM Movies WHERE Id = @Id LIMIT 1",
            new { _testMovie.Id });
    }
}