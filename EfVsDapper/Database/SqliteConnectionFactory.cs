using System.Data;
using System.Data.Common;
using System.Data.SQLite.EF6;
using Dapper;
using Microsoft.Data.Sqlite;

namespace EfVsDapper.Database;

public class SqliteConnectionFactory:DbProviderFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));
        
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync ()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        return connection;
    }
}