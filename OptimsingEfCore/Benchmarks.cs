using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using EfVsDapper;
using EfVsDapper.Database;
using Microsoft.EntityFrameworkCore;

namespace OptimsingEfCore;

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
        return _dbConnection.QuerySingleOrDefault<Movie>("""
SELECT * FROM Movies WHERE Id = @Id LIMIT 1
""",
            new { _testMovie.Id });
    }
    
    private static readonly Func<MoviesContext, Guid, Movie?> SingleMovieAsync = 
        EF.CompileQuery((MoviesContext context, Guid id) => context.Movies.SingleOrDefault(x => x.Id == id));
    
    [Benchmark]
    public Movie? EF_Single_CompileQuery ()
    {
        return SingleMovieAsync(_moviesContext, _testMovie.Id);
    }
    
    private static readonly Func<MoviesContext, Guid, Movie?> FirstMovieAsync = 
        EF.CompileQuery((MoviesContext context, Guid id) => context.Movies.FirstOrDefault(x => x.Id == id));
    
    [Benchmark]
    public Movie? EF_First_CompileQuery ()
    {
        return FirstMovieAsync(_moviesContext, _testMovie.Id);
    }
    
    

}