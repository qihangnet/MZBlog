using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.Extensions
{
    public static class TagExtension
    {
        private static DB.AutoBox _db;

        public static void SetupDb(DB.AutoBox db)
        {
            if (_db == null)
                _db = db;
        }

        public static Tag AsTag(this string slug)
        {
            return _db.SelectKey<Tag>(DBTableNames.Tags, slug);
        }

        public static IEnumerable<Tag> Project(this IEnumerable<string> input)
        {
            var tags = from slug in input
                       select _db.SelectKey<Tag>(DBTableNames.Tags, slug);
            return tags;
        }
    }
}