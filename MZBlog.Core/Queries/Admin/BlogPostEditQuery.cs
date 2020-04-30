using Dapper;
using Dapper.Extensions;
using MediatR;
using Microsoft.Data.Sqlite;
using MZBlog.Core.Entities;

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
            var tags = _conn.Query<string>("SELECT t.Name FROM BlogPostTags p INNER JOIN Tag t ON t.Slug=p.TagSlug WHERE p.BlogPostId=@Id", new { post.Id });
            post.Tags = tags;
            return new BlogPostEditViewModel { BlogPost = post };
        }
    }
}