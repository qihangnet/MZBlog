using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagDetailViewProjection : IViewProjection<string, Tag>
    {
        private readonly DB.AutoBox _db;

        public TagDetailViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public Tag Project(string input)
        {
            var tag = _db.SelectKey<Tag>(DBTableNames.Tags, input);
            return tag;
        }
    }

    public class TagsViewProjection : IViewProjection<IEnumerable<string>, IEnumerable<Tag>>
    {
        private readonly DB.AutoBox _db;

        public TagsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public IEnumerable<Tag> Project(IEnumerable<string> input)
        {
            var tags = from slug in input
                       select _db.SelectKey<Tag>(DBTableNames.Tags, slug);
            return tags;
        }
    }
}