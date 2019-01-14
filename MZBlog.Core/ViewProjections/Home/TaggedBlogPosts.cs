using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TaggedBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public string Tag { get; set; }
    }

    public class TaggedBlogPostsQuery : IRequest<TaggedBlogPostsViewModel>
    {
        public string Tag { get; set; }
    }

    public class TaggedBlogPostsViewProjection : RequestHandler<TaggedBlogPostsQuery, TaggedBlogPostsViewModel>
    {
        private readonly DB.AutoBox _db;

        public TaggedBlogPostsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }
        protected override TaggedBlogPostsViewModel Handle(TaggedBlogPostsQuery request)
        {
            var posts = (from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                         where p.IsPublished && p.Tags.Contains(request.Tag)
                         orderby p.PubDate descending
                         select p)
                     .ToList();
            if (posts.Count == 0)
                return null;
            var tagName = _db.SelectKey<Tag>(DBTableNames.Tags, request.Tag).Name;
            return new TaggedBlogPostsViewModel
            {
                Posts = posts,
                Tag = tagName
            };
        }
    }
}