using Dapper;
using Dapper.Extensions;
using MediatR;
using Microsoft.Data.Sqlite;
using MZBlog.Core.Entities;
using System.Linq;

namespace MZBlog.Core.Queries.Home
{
    public class BlogPostDetailsQuery : IRequest<BlogPostDetailsViewModel>
    {
        public string Permalink { get; set; }
    }

    public class BlogPostDetailsViewModel
    {
        public BlogPost BlogPost { get; set; }

        public BlogComment[] Comments { get; set; }
    }

    public class BlogPostDetailsViewProjection : RequestHandler<BlogPostDetailsQuery, BlogPostDetailsViewModel>
    {
        private readonly SqliteConnection _conn;

        public BlogPostDetailsViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override BlogPostDetailsViewModel Handle(BlogPostDetailsQuery request)
        {
            var post = _conn.QueryFirstOrDefault<BlogPost>("SELECT * FROM BlogPost WHERE TitleSlug=@Permalink", request);
            if (post == null)
                return null;

            post.ViewCount++;
            _conn.Update(post);

            var comments = _conn.Query<BlogComment>("SELECT * FROM BlogComment WHERE PostId=@Id ORDER BY CreatedTime", new { post.Id })
                                    .ToArray();

            var tags = _conn.Query<string>("SELECT t.Name FROM BlogPostTags p INNER JOIN Tag t ON t.Slug=p.TagSlug WHERE p.BlogPostId=@Id", new { post.Id });
            post.Tags = tags;

            return new BlogPostDetailsViewModel
            {
                BlogPost = post,
                Comments = comments
            };
        }
    }
}