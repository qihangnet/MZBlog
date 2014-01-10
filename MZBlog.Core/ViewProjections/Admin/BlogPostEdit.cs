using MongoDB.Driver.Linq;
using MZBlog.Core.Documents;
using System.Linq;

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
        private readonly MongoCollections _collections;

        public BlogPostEditViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public BlogPostEditViewModel Project(BlogPostEditBindingModel input)
        {
            var post = _collections.BlogPostCollection
                                 .AsQueryable()
                                 .FirstOrDefault(b => b.Id == input.PostId);

            return new BlogPostEditViewModel { BlogPost = post };
        }
    }
}