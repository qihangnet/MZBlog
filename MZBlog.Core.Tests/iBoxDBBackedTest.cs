using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System;
using System.IO;

namespace MZBlog.Core.Tests
{
    public class iBoxDBBackedTest
    {
        private static readonly object _lock = new object();
        protected static DB.AutoBox OpenTestDb()
        {
            var dbPath = Path.Combine("ibox", Guid.NewGuid().ToString());
            lock (_lock)
            {

                if (!Directory.Exists(dbPath))
                {
                    Directory.CreateDirectory(dbPath);
                }
            }
            
            var server = new DB(dbPath);
            server.GetConfig().EnsureTable<Author>(DBTableNames.Authors, "Id");
            return server.Open();
        }
    }
}