using System.Data;
using System.Data.Entity;
using BenchmarkDotNet.Attributes;
using Dapper;
using EfVsDapper.Database;
using Microsoft.Data.Sqlite;

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
        var dbConnectionFactory = new SqliteConnectionFactory("");

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
    public async Task<Movie?> EF_Find ()
    {
        return await _moviesContext.Movies.FindAsync(_testMovie.Id);
    }

    [Benchmark]
    public async Task<Movie?> EF_Single ()
    {
        return await _moviesContext.Movies.SingleOrDefaultAsync(x => x.Id == _testMovie.Id);
    }

    [Benchmark]
    public async Task<Movie?> EF_First ()
    {
        return await _moviesContext.Movies.FirstOrDefaultAsync(x => x.Id == _testMovie.Id);
    }

    [Benchmark]
    public async Task<Movie> Dapper_GetById ()
    {
        return await _dbConnection.QuerySingleOrDefaultAsync<Movie>("SELECT * FROM Movies WHERE Id = @Id LIMIT 1",
            new { _testMovie.Id });
    }
}