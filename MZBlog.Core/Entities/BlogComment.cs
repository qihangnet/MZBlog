using Dapper.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace MZBlog.Core.Entities
{
    public class BlogComment
    {
        [ExplicitKey]
        public string Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        public string Email { get; set; }

        public string SiteUrl { get; set; }

        public DateTime CreatedTime { get; set; }

        public string PostId { get; set; }

        public string IPAddress { get; set; }

        [Computed]
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