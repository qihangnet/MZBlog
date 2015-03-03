using iBoxDB.LocalServer;
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
        private readonly DB.AutoBox _db;

        public TaggedBlogPostsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public TaggedBlogPostsViewModel Project(TaggedBlogPostsBindingModel input)
        {
            var posts = _db.Select<BlogPost>("from " + DBTableNames.BlogPosts + " where IsPublished==true order by PubDate desc")
                     .Where(b => b.Tags.Any(t => t.Slug == input.Tag))
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