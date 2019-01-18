using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using Dapper;
using Dapper.Extensions;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class BlogPostEditBindingModel : IRequest<BlogPostEditViewModel>
    {
        public string PostId { get; set; }
    }

    public class BlogPostEditViewModel
    {
        public BlogPost BlogPost { get; set; }
    }

    public class BlogPostEditViewProjection : RequestHandler<BlogPostEditBindingModel, BlogPostEditViewModel>
    {
        private readonly SqliteConnection _conn;

        public BlogPostEditViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override BlogPostEditViewModel Handle(BlogPostEditBindingModel request)
        {
            var post = _conn.Get<BlogPost>(request.PostId);

            return new BlogPostEditViewModel { BlogPost = post };
        }
    }
}