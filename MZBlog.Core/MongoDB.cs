using MongoDB.Driver;
using MZBlog.Core.Documents;

namespace MZBlog.Core
{
    public class MongoCollections
    {
        private readonly MongoDatabase _database;

        public MongoCollections(MongoDatabase database)
        {
            _database = database;
        }

        public MongoCollection<BlogComment> BlogCommentCollection
        {
            get
            {
                return _database.GetCollection<BlogComment>("BlogComments");
            }
        }

        public MongoCollection<BlogPost> BlogPostCollection
        {
            get
            {
                return _database.GetCollection<BlogPost>("BlogPosts");
            }
        }

        public MongoCollection<Author> AuthorCollection
        {
            get
            {
                return _database.GetCollection<Author>("Authors");
            }
        }

        public MongoCollection<SpamHash> SpamHashCollection
        {
            get
            {
                return _database.GetCollection<SpamHash>("SpamHashes");
            }
        }
    }
}