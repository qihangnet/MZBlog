using Dapper;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace MZBlog.Core.Tests
{
    public class SqliteBackedTest
    {
        protected SqliteConnection GetMemorySqliteConnection()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            return conn;
        }

        protected async Task<int> CreateAuthorTable(SqliteConnection conn)
        {
            
            await conn.ExecuteAsync(@"CREATE TABLE Author (
                                Id varchar(24) PRIMARY KEY NOT NULL,
                                Email varchar(256) NOT NULL,
                                HashedPassword varchar(200) NOT NULL,
                                DisplayName nvarchar(50) NOT NULL,
                                CreatedUTC datetime NOT NULL
                            );");
            return await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM sqlite_master WHERE type='table'");
        }
    }
}