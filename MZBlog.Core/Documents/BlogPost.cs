using MZBlog.Core.Extensions;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace MZBlog.Core.Documents
{
    public class BlogPost
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string TitleSlug { get; set; }

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

        public DateTime PubDate { get; set; }

        public DateTime DateUTC { get; set; }

        public Tag[] Tags { get; set; }

        public string AuthorDisplayName { get; set; }

        public string AuthorEmail { get; set; }

        public bool IsPublished
        {
            get { return PubDate <= DateTime.UtcNow && Status == PublishStatus.Published; }
        }

        public string GetLink()
        {
            return "/{0}/{1}".FormatWith(PubDate.ToString("yyyy/MM", CultureInfo.InvariantCulture), TitleSlug);
        }
    }

    public static class BlogPostExtensions
    {
        public static BlogPost ToPublishedBlogPost(this BlogPost blogPost)
        {
            blogPost.PubDate = DateTime.UtcNow.AddDays(-1);
            blogPost.Status = PublishStatus.Published;
            return blogPost;
        }
    }
}