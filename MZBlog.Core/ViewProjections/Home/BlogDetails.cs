using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class BlogPostDetailsBindingModel
    {
        public string Permalink { get; set; }
    }

    public class BlogPostDetailsViewModel
    {
        public BlogPost BlogPost { get; set; }

        public BlogComment[] Comments { get; set; }
    }

    public class BlogPostDetailsViewProjection : IViewProjection<BlogPostDetailsBindingModel, BlogPostDetailsViewModel>
    {
        private readonly DB.AutoBox _db;

        public BlogPostDetailsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public BlogPostDetailsViewModel Project(BlogPostDetailsBindingModel input)
        {
            var post = _db.Select<BlogPost>("from " + DBTableNames.BlogPosts + " where TitleSlug==" + input.Permalink).FirstOrDefault();
            if (post == null)
                return null;
            post.ViewCount++;
            _db.Update(DBTableNames.BlogPosts, post);

            var comments = _db.Select<BlogComment>("from " + DBTableNames.BlogComments + " where PostId == " + post.Id)
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