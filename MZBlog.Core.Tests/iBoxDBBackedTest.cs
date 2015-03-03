using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System;

namespace MZBlog.Core.Tests
{
    public class iBoxDBBackedTest : IDisposable
    {
        protected readonly DB.AutoBox _db;

        public iBoxDBBackedTest()
        {
            var server = new DB();
            server.GetConfig().EnsureTable<Author>(DBTableNames.Authors, "Id");
            _db = server.Open();
        }

        public void Dispose()
        {
            _db.GetDatabase().Dispose();
        }
    }
}