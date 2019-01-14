using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;

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
        private readonly DB.AutoBox _db;

        public BlogPostEditViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        protected override BlogPostEditViewModel Handle(BlogPostEditBindingModel request)
        {
            var post = _db.SelectKey<BlogPost>(DBTableNames.BlogPosts, request.PostId);

            return new BlogPostEditViewModel { BlogPost = post };
        }
    }
}