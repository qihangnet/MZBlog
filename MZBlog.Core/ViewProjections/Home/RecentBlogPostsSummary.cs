using iBoxDB.LocalServer;
using MediatR;
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

    public class RecentBlogPostSummaryBindingModel : IRequest<RecentBlogPostSummaryViewModel>
    {
        public int Page { get; set; }
    }

    public class RecentBlogPostSummaryViewProjection : RequestHandler<RecentBlogPostSummaryBindingModel, RecentBlogPostSummaryViewModel>
    {
        private readonly DB.AutoBox _db;

        public RecentBlogPostSummaryViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        protected override RecentBlogPostSummaryViewModel Handle(RecentBlogPostSummaryBindingModel request)
        {
            var titles = (from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                          where p.IsPublished
                          orderby p.PubDate descending
                          select new BlogPostSummary
                          {
                              Title = p.Title,
                              Link = p.GetLink()
                          }
                          )
                .Take(request.Page)
                .ToList()
                .AsReadOnly();

            return new RecentBlogPostSummaryViewModel { BlogPostsSummaries = titles };
        }
    }
}