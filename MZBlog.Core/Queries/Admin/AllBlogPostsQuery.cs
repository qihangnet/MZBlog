using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace MZBlog.Core.Queries.Admin
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

    public class AllBlogPostsQuery : IRequest<AllBlogPostsViewModel>
    {
        public AllBlogPostsQuery()
        {
            Page = 1;
            Take = 10;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class AllBlogPostViewProjection : RequestHandler<AllBlogPostsQuery, AllBlogPostsViewModel>
    {
        private readonly SqliteConnection _conn;

        public AllBlogPostViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override AllBlogPostsViewModel Handle(AllBlogPostsQuery request)
        {
            var skip = (request.Page - 1) * request.Take;
            var list = _conn.Query<BlogPost>($"select * from BlogPost order by CreatedUTC desc limit {request.Take + 1} OFFSET {skip}");

            var pagedPosts = list.Take(request.Take);
            var hasNextPage = list.Count() > request.Take;

            return new AllBlogPostsViewModel
            {
                Posts = pagedPosts,
                Page = request.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}