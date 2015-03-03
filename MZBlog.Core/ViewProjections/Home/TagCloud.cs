using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagCloudBindingModel
    {
        public int Threshold { get; set; }

        public int Take { get; set; }

        public TagCloudBindingModel()
        {
            Threshold = 1;
            Take = int.MaxValue;
        }
    }

    public class TagCloudViewModel
    {
        public Dictionary<Tag, int> Tags { get; set; }
    }

    public class TagCloudViewProjection : IViewProjection<TagCloudBindingModel, TagCloudViewModel>
    {
        private readonly DB.AutoBox _db;

        public TagCloudViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public TagCloudViewModel Project(TagCloudBindingModel input)
        {
            var result = new Dictionary<Tag, int>();
            var tags = _db.Select<Tag>("from " + DBTableNames.Tags);
            var blogPosts = _db.Select<BlogPost>("from " + DBTableNames.BlogPosts);
            foreach (var tag in tags)
            {
                result.Add(tag, blogPosts.Count(w => w.Tags.Select(s => s.Slug).Contains(tag.Slug)));
            }
            return new TagCloudViewModel
            {
                Tags = result
            };
        }
    }
}