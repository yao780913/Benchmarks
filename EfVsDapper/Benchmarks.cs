using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using EfVsDapper.Database;
using Microsoft.Diagnostics.Runtime;

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
    
    // [Benchmark]
    // public Movie? EF_Find ()
    // {
    //     return _moviesContext.Movies.Find(_testMovie.Id);
    // }
    //
    // [Benchmark]
    // public Movie? EF_Single ()
    // {
    //     return _moviesContext.Movies.SingleOrDefault(x => x.Id == _testMovie.Id);
    // }
    //
    // [Benchmark]
    // public Movie? EF_First ()
    // {
    //     return _moviesContext.Movies.FirstOrDefault(x => x.Id == _testMovie.Id);
    // }
    //
    // [Benchmark]
    // public Movie? Dapper_GetById ()
    // {
    //     return _dbConnection.QuerySingleOrDefault<Movie>("SELECT * FROM Movies WHERE Id = @Id LIMIT 1",
    //         new { _testMovie.Id });
    // }

//     [Benchmark]
//     public Movie EF_Add_Delete ()
//     {
//         var movie = new Movie
//         {
//             Id = Guid.NewGuid(),
//             Title = "Test Movie2",
//             YearOfRelease = 2015
//         };
//
//         _moviesContext.Movies.Add(movie);
//         _moviesContext.SaveChanges();
//
//         _moviesContext.Movies.Remove(movie);
//         _moviesContext.SaveChanges();
//
//         return movie;
//     }
//
//     [Benchmark]
//     public Movie Dapper_Add_Delete ()
//     {
//         var movie = new Movie
//         {
//             Id = Guid.NewGuid(),
//             Title = "Test Movie2",
//             YearOfRelease = 2015
//         };
//
//         _dbConnection.Execute(@"
// INSERT INTO Movies(Id, Title, YearOfRelease)
// VALUES (@Id, @Title, @YearOfRelease)", movie);
//
//         _dbConnection.Execute("DELETE FROM Movies WHERE Id = @Id", movie);
//
//         return movie;
//     }

//     [Benchmark]
//     public Movie EF_Update ()
//     {
//         _testMovie.YearOfRelease = _random.Next();
//         _moviesContext.Movies.Update(_testMovie);
//         _moviesContext.SaveChanges();
//
//         return _testMovie;
//     }
//     
//     [Benchmark]
//     public Movie Dapper_Update ()
//     {
//         _testMovie.YearOfRelease = _random.Next();
//         _dbConnection.Execute(@"
// UPDATE Movies
// SET YearOfRelease = @YearOfRelease
// WHERE Id = @Id", _testMovie);
//
//         return _testMovie;
//     }

    [Benchmark]
    public List<Movie> EF_Filter ()
    {
        var movies = _moviesContext.Movies.Where(m => m.YearOfRelease == 1993);
        return movies.ToList();
    }
    
    [Benchmark]
    public List<Movie> Dapper_Filter ()
    {
        var movies = _dbConnection.Query<Movie>("SELECT * FROM Movies WHERE YearOfRelease = 1993");
        return movies.ToList();
    }
}