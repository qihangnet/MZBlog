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
            var posts = (from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                         where p.IsPublished && p.Tags.Contains(input.Tag)
                         orderby p.PubDate descending
                         select p)
                     .ToList();
            if (posts.Count == 0)
                return null;
            var tagName = _db.SelectKey<Tag>(DBTableNames.Tags, posts.First().Tags[0]).Name;
            return new TaggedBlogPostsViewModel
            {
                Posts = posts,
                Tag = tagName
            };
        }
    }
}