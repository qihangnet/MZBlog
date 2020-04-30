using DbUp;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace MZBlog.Web.Data
{
    public class DbSetup
    {
        private readonly IConfiguration _configuration;

        public DbSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static DbSetup UseConfiguration(IConfiguration configuration)
        {
            return new DbSetup(configuration);
        }

        public bool Upgrade(string dbFile)
        {
            var runtimeAppDataPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "App_Data");
            if (!Directory.Exists(runtimeAppDataPath))
            {
                Directory.CreateDirectory(runtimeAppDataPath);
            }

            var dbPath = Path.Combine(runtimeAppDataPath, dbFile);
            var connString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();

            var upgrader = DeployChanges.To
                                        .SQLiteDatabase(connString)
                                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                                        .LogToConsole()
                                        .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[DbUp] {result.Error}");
                Console.ResetColor();
                return false;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[DbUp] Success!");
            Console.ResetColor();
            return true;
        }
    }
}
