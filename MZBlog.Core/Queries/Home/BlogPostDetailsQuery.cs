using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using System.Linq;
using Dapper;
using Dapper.Extensions;

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
            var post = _conn.QuerySingle<BlogPost>("select * from BlogPost where TitleSlug=@Permalink", request);
            if (post == null)
                return null;

            post.ViewCount++;
            _conn.Update(post);

            var comments = _conn.Query<BlogComment>("select * from BlogComment where PostId=@Id order by CreatedTime", new { post.Id })
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