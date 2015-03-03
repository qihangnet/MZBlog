using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class RecentBlogPostSummaryViewModel
    {
        public IEnumerable<BlogPostSummary> BlogPostsSummaries { get; set; }
    }

    public class BlogPostSummary
    {
        public string Title { get; set; }

        public string Link { get; set; }
    }

    public class RecentBlogPostSummaryBindingModel
    {
        public int Page { get; set; }
    }

    public class RecentBlogPostSummaryViewProjection : IViewProjection<RecentBlogPostSummaryBindingModel, RecentBlogPostSummaryViewModel>
    {
        private readonly DB.AutoBox _db;

        public RecentBlogPostSummaryViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public RecentBlogPostSummaryViewModel Project(RecentBlogPostSummaryBindingModel input)
        {
            var titles = _db.Select<BlogPost>("from " + DBTableNames.BlogPosts + " where IsPublished==true order by PubDate desc limit 0," + input.Page)
                     .Select(b => new BlogPostSummary()
                     {
                         Title = b.Title,
                         Link = b.GetLink()
                     })
                     .ToList().AsReadOnly();

            return new RecentBlogPostSummaryViewModel { BlogPostsSummaries = titles };
        }
    }
}