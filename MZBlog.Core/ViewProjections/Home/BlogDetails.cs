using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Linq;
using MediatR;

namespace MZBlog.Core.ViewProjections.Home
{
    public class BlogPostDetailsQuery:IRequest<BlogPostDetailsViewModel>
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
        private readonly DB.AutoBox _db;

        public BlogPostDetailsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        protected override BlogPostDetailsViewModel Handle(BlogPostDetailsQuery request)
        {
            var post = _db.Select<BlogPost>("from " + DBTableNames.BlogPosts + " where TitleSlug==?", request.Permalink).FirstOrDefault();
            if (post == null)
                return null;
            post.ViewCount++;
            _db.Update(DBTableNames.BlogPosts, post);

            var comments = _db.Select<BlogComment>("from " + DBTableNames.BlogComments + " where PostId ==?", post.Id)
                                    .OrderBy(o => o.CreatedTime)
                                    .ToArray();

            return new BlogPostDetailsViewModel
            {
                BlogPost = post,
                Comments = comments
            };
        }
    }
}