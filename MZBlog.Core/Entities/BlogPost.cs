using Dapper.Extensions;
using MZBlog.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MZBlog.Core.Entities
{
    public class BlogPost
    {
        [ExplicitKey]
        public string Id { get; set; }

        public string Title { get; set; }

        public string TitleSlug { get; set; }

        [Computed]
        public string Description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content))
                    return string.Empty;
                var des = Regex.Replace(Content, "<[^>]+>", string.Empty).Trim();
                var length = des.Length < 240 ? des.Length : 240;
                des = des.Substring(0, length) + " ...";
                return des;
            }
        }

        public int ViewCount { get; set; }

        public string MarkDown { get; set; }

        public string Content { get; set; }

        public PublishStatus Status { get; set; }

        public DateTime PublishUTC { get; set; }

        public DateTime CreatedUTC { get; set; }

        [Computed]
        public IEnumerable<string> Tags { get; set; }

        public string AuthorDisplayName { get; set; }

        public string AuthorEmail { get; set; }

        [Computed]
        public bool IsPublished
        {
            get { return PublishUTC <= DateTime.UtcNow && Status == PublishStatus.Published; }
        }

        public string GetLink()
        {
            return "/{0}/{1}".FormatWith(PublishUTC.ToString("yyyy/MM", CultureInfo.InvariantCulture), TitleSlug);
        }
    }

    public static class BlogPostExtensions
    {
        public static BlogPost ToPublishedBlogPost(this BlogPost blogPost)
        {
            blogPost.PublishUTC = DateTime.UtcNow.AddDays(-1);
            blogPost.Status = PublishStatus.Published;
            return blogPost;
        }
    }
}