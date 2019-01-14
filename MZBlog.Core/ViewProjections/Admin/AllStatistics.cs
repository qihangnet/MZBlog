using iBoxDB.LocalServer;
using MediatR;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AllStatisticsViewModel
    {
        public long PostsCount { get; set; }

        public long CommentsCount { get; set; }

        public int TagsCount { get; set; }
    }

    public class AllStatisticsBindingModel : IRequest<AllStatisticsViewModel>
    {
        public AllStatisticsBindingModel()
        {
            TagThreshold = 1;
        }

        public int TagThreshold { get; set; }
    }

    public class AllStatisticsViewProjection : RequestHandler<AllStatisticsBindingModel, AllStatisticsViewModel>
    {
        private readonly DB.AutoBox _db;

        public AllStatisticsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        protected override AllStatisticsViewModel Handle(AllStatisticsBindingModel request)
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