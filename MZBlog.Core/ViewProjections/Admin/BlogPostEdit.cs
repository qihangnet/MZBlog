using iBoxDB.LocalServer;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class BlogPostEditBindingModel
    {
        public string PostId { get; set; }
    }

    public class BlogPostEditViewModel
    {
        public BlogPost BlogPost { get; set; }
    }

    public class BlogPostEditViewProjection : IViewProjection<BlogPostEditBindingModel, BlogPostEditViewModel>
    {
        private readonly DB.AutoBox _db;

        public BlogPostEditViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public BlogPostEditViewModel Project(BlogPostEditBindingModel input)
        {
            var post = _db.SelectKey<BlogPost>(DBTableNames.BlogPosts, input.PostId);

            return new BlogPostEditViewModel { BlogPost = post };
        }
    }
}