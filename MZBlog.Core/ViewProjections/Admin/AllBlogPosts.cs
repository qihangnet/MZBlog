using iBoxDB.LocalServer;
using MediatR;
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

    public class AllBlogPostsBindingModel : IRequest<AllBlogPostsViewModel>
    {
        public AllBlogPostsBindingModel()
        {
            Page = 1;
            Take = 10;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class AllBlogPostViewProjection : RequestHandler<AllBlogPostsBindingModel, AllBlogPostsViewModel>
    {
        private readonly DB.AutoBox _db;

        public AllBlogPostViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        protected override AllBlogPostsViewModel Handle(AllBlogPostsBindingModel request)
        {
            var skip = (request.Page - 1) * request.Take;

            var posts = (from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                         orderby p.DateUTC descending
                         select p)
                        .Skip(skip)
                        .Take(request.Take + 1)
                        .ToList()
                        .AsReadOnly();

            var pagedPosts = posts.Take(request.Take);
            var hasNextPage = posts.Count > request.Take;

            return new AllBlogPostsViewModel
            {
                Posts = pagedPosts,
                Page = request.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}