using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace MZBlog.Core.Queries.Admin
{
    public class AllBlogCommentsQuery : IRequest<AllBlogCommentsViewModel>
    {
        public AllBlogCommentsQuery()
        {
            Page = 1;
            Take = 20;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class AllBlogCommentsViewModel
    {
        public IEnumerable<BlogComment> Comments { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPrevPage
        {
            get
            {
                return Page > 1;
            }
        }

        public int Page { get; set; }
    }

    public class BlogCommentsViewProjection : RequestHandler<AllBlogCommentsQuery, AllBlogCommentsViewModel>
    {
        private readonly SqliteConnection _conn;

        public BlogCommentsViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override AllBlogCommentsViewModel Handle(AllBlogCommentsQuery request)
        {
            var skip = (request.Page - 1) * request.Take;
            var list = _conn.Query<BlogComment>($"select * from BlogComment order by CreatedTime desc limit {request.Take + 1} OFFSET {skip}");

            var pagedComments = list.Take(request.Take);
            var hasNextPage = list.Count() > request.Take;

            return new AllBlogCommentsViewModel
            {
                Comments = pagedComments,
                Page = request.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}