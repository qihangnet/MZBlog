using MongoDB.Driver.Linq;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class RecentBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public int Page { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPrevPage
        {
            get
            {
                return Page > 1;
            }
        }
    }

    public class RecentBlogPostsBindingModel
    {
        public RecentBlogPostsBindingModel()
        {
            Page = 1;
            Take = 20;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class RecentBlogPostViewProjection : IViewProjection<RecentBlogPostsBindingModel, RecentBlogPostsViewModel>
    {
        private readonly MongoCollections _collections;

        public RecentBlogPostViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public RecentBlogPostsViewModel Project(RecentBlogPostsBindingModel input)
        {
            var posts = _collections.BlogPostCollection.AsQueryable()
                     .Where(BlogPost.IsPublished)
                     .OrderByDescending(b => b.PubDate)
                     .TakePage(input.Page, pageSize: input.Take + 1)
                     .ToList();
            var pagedPosts = posts.Take(input.Take).ToList();
            var hasNextPage = posts.Count > input.Take;

            return new RecentBlogPostsViewModel
            {
                Posts = pagedPosts,
                Page = input.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}