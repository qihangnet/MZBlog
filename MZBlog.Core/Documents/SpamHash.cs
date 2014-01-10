using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MZBlog.Core.Documents
{
    public class SpamHash
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PostKey { get; set; }

        public string Hash { get; set; }

        public bool Pass { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}