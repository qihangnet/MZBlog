using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Entities;
using Dapper;
using Dapper.Extensions;

namespace MZBlog.Core.Queries.Admin
{
    public class BlogPostEditQuery : IRequest<BlogPostEditViewModel>
    {
        public string PostId { get; set; }
    }

    public class BlogPostEditViewModel
    {
        public BlogPost BlogPost { get; set; }
    }

    public class BlogPostEditViewProjection : RequestHandler<BlogPostEditQuery, BlogPostEditViewModel>
    {
        private readonly SqliteConnection _conn;

        public BlogPostEditViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override BlogPostEditViewModel Handle(BlogPostEditQuery request)
        {
            var post = _conn.Get<BlogPost>(request.PostId);

            return new BlogPostEditViewModel { BlogPost = post };
        }
    }
}