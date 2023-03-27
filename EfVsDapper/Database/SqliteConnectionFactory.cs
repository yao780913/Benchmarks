using System.Data.Common;
using System.Data.Entity.Infrastructure;
using Dapper;
using Microsoft.Data.Sqlite;

namespace EfVsDapper.Database;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));
        
        _connectionString = connectionString;
    }
    
    public DbConnection CreateConnection (string nameOrConnectionString)
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        return connection;
    }
}