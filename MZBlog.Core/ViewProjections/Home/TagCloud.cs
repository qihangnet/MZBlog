using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;

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
        public IEnumerable<Tag> Tags { get; set; }
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
            var tags = _db.Select<Tag>("from " + DBTableNames.Tags + " order by PostCount desc");

            return new TagCloudViewModel
            {
                Tags = tags
            };
        }
    }
}