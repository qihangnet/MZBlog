using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace MZBlog.Core.Documents
{
    public class BlogComment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        public string Email { get; set; }

        public string SiteUrl { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedTime { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; }

        public string IPAddress { get; set; }

        public string Avatar
        {
            get
            {
                var imgUrl = string.Format("https://secure.gravatar.com/avatar/{0}.png?s={1}&d={2}&r=g", Hasher.GetMd5Hash(Email), 60, "mm");
                return imgUrl;
            }
        }
    }
}