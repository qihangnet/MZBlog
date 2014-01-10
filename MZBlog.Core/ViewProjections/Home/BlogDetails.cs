using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
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
        private readonly MongoCollections _collections;

        public BlogPostDetailsViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public BlogPostDetailsViewModel Project(BlogPostDetailsBindingModel input)
        {
            var postsCollection = _collections.BlogPostCollection;
            var post = postsCollection.AsQueryable()
                                 .FirstOrDefault(b => b.TitleSlug == input.Permalink);
            if (post == null)
                return null;

            postsCollection.Update(Query.EQ("TitleSlug", input.Permalink), Update.Inc("ViewCount", 1));

            var comments = _collections.BlogCommentCollection
                                    .AsQueryable()
                                    .Where(w => w.PostId == post.Id)
                                    .ToList()
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