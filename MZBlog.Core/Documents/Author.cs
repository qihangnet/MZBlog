using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MZBlog.Core.Documents
{
    public class Author
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string HashedPassword { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string[] Roles { get; set; }
    }
}