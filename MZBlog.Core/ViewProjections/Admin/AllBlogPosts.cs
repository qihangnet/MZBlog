using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
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
        private readonly DB.AutoBox _db;

        public AllBlogPostViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public AllBlogPostsViewModel Project(AllBlogPostsBindingModel input)
        {
            var skip = (input.Page - 1) * input.Take;

            var posts = (from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                         orderby p.DateUTC descending
                         select p)
                        .Skip(skip)
                        .Take(input.Take + 1)
                        .ToList()
                        .AsReadOnly();

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