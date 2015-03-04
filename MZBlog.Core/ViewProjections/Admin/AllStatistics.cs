using iBoxDB.LocalServer;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AllStatisticsViewModel
    {
        public long PostsCount { get; set; }

        public long CommentsCount { get; set; }

        public int TagsCount { get; set; }
    }

    public class AllStatisticsBindingModel
    {
        public AllStatisticsBindingModel()
        {
            TagThreshold = 1;
        }

        public int TagThreshold { get; set; }
    }

    public class AllStatisticsViewProjection : IViewProjection<AllStatisticsBindingModel, AllStatisticsViewModel>
    {
        private readonly DB.AutoBox _db;

        public AllStatisticsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public AllStatisticsViewModel Project(AllStatisticsBindingModel input)
        {
            var postCount = _db.SelectCount("from " + DBTableNames.BlogPosts);
            if (postCount == 0)
                return new AllStatisticsViewModel();

            var stat = new AllStatisticsViewModel
            {
                PostsCount = postCount,
                CommentsCount = _db.SelectCount("from " + DBTableNames.BlogComments)
            };

            stat.TagsCount = (int)_db.SelectCount("from " + DBTableNames.Tags);

            return stat;
        }
    }
}