using MongoDB.Driver.Linq;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TaggedBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public string Tag { get; set; }
    }

    public class TaggedBlogPostsBindingModel
    {
        public string Tag { get; set; }
    }

    public class TaggedBlogPostsViewProjection : IViewProjection<TaggedBlogPostsBindingModel, TaggedBlogPostsViewModel>
    {
        private readonly MongoCollections _collections;

        public TaggedBlogPostsViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public TaggedBlogPostsViewModel Project(TaggedBlogPostsBindingModel input)
        {
            var posts = _collections.BlogPostCollection.AsQueryable()
                     .Where(BlogPost.IsPublished)
                     .Where(b => b.Tags.Any(t => t.Slug == input.Tag))
                     .OrderByDescending(b => b.PubDate)
                     .Take(10)
                     .ToList();
            if (posts.Count == 0)
                return null;
            var tagName = posts.First().Tags.First(w => w.Slug == input.Tag).Name;
            return new TaggedBlogPostsViewModel
            {
                Posts = posts,
                Tag = tagName
            };
        }
    }
}