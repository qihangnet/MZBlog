using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;
using MediatR;
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

    public class RecentBlogPostsQuery: IRequest<RecentBlogPostsViewModel>
    {
        public RecentBlogPostsQuery()
        {
            Page = 1;
            Take = 20;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class RecentBlogPostsQueryHandler : RequestHandler<RecentBlogPostsQuery, RecentBlogPostsViewModel>
    {
        private readonly DB.AutoBox _db;

        public RecentBlogPostsQueryHandler(DB.AutoBox db)
        {
            _db = db;
        }

        protected override RecentBlogPostsViewModel Handle(RecentBlogPostsQuery request)
        {
            var skip = (request.Page - 1) * request.Take;
            var posts = (from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                         where p.IsPublished
                         orderby p.PubDate descending
                         select p)
                         .Skip(skip)
                         .Take(request.Take + 1)
                         .ToList()
                         .AsReadOnly();

            var pagedPosts = posts.Take(request.Take).ToList();
            var hasNextPage = posts.Count > request.Take;

            return new RecentBlogPostsViewModel
            {
                Posts = pagedPosts,
                Page = request.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}