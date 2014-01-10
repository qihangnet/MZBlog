using MongoDB.Driver.Linq;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AllBlogPostsViewModel
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

    public class AllBlogPostsBindingModel
    {
        public AllBlogPostsBindingModel()
        {
            Page = 1;
            Take = 10;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class AllBlogPostViewProjection : IViewProjection<AllBlogPostsBindingModel, AllBlogPostsViewModel>
    {
        private readonly MongoCollections _collections;

        public AllBlogPostViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public AllBlogPostsViewModel Project(AllBlogPostsBindingModel input)
        {
            var posts = _collections.BlogPostCollection
                     .AsQueryable()
                     .OrderByDescending(b => b.DateUTC)
                     .TakePage(input.Page, pageSize: input.Take + 1)
                     .ToList();
            var pagedPosts = posts.Take(input.Take);
            var hasNextPage = posts.Count > input.Take;

            return new AllBlogPostsViewModel
            {
                Posts = pagedPosts,
                Page = input.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}