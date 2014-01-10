using MongoDB.Driver.Linq;
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
        private readonly MongoCollections _collections;

        public RecentBlogPostSummaryViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public RecentBlogPostSummaryViewModel Project(RecentBlogPostSummaryBindingModel input)
        {
            var titles = _collections.BlogPostCollection.AsQueryable()
                     .Where(BlogPost.IsPublished)
                     .OrderByDescending(b => b.PubDate)
                     .Select(b => new BlogPostSummary()
                     {
                         Title = b.Title,
                         Link = b.GetLink()
                     })
                     .Take(10)
                     .ToList();

            return new RecentBlogPostSummaryViewModel { BlogPostsSummaries = titles };
        }
    }
}