using iBoxDB.LocalServer;
using MZBlog.Core.Documents;

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
    }
}