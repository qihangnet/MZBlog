using MongoDB.Driver;
using System;

namespace MZBlog.Core.Tests
{
    public class MongoDBBackedTest : IDisposable
    {
        private MongoCollections _cols;

        public MongoDBBackedTest()
        {
            Collections.AuthorCollection.Drop();
            Collections.BlogPostCollection.Drop();
            Collections.BlogCommentCollection.Drop();
            Collections.SpamHashCollection.Drop();
        }

        protected MongoCollections Collections
        {
            get
            {
                if (_cols == null)
                {
                    var client = new MongoClient("mongodb://localhost");
                    var _db = client.GetServer().GetDatabase("MZBlog_Tests");
                    _cols = new MongoCollections(_db);
                }
                return _cols;
            }
        }

        public void Dispose()
        {
            Collections.AuthorCollection.Drop();
            Collections.BlogPostCollection.Drop();
            Collections.BlogCommentCollection.Drop();
            Collections.SpamHashCollection.Drop();
        }
    }
}