using iBoxDB.LocalServer;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MZBlog.Core.Documents;
using System.Linq;

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
            var postCount=_db.SelectCount("from " + DBTableNames.BlogPosts);
            if (postCount == 0)
                return new AllStatisticsViewModel();

            var stat = new AllStatisticsViewModel
            {
                PostsCount = postCount,
                CommentsCount = _db.SelectCount("from " + DBTableNames.BlogComments)
            };
            //TODO Tag统计
            //var tags = from post in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
            //           select post.Tags;
            //stat.TagsCount = result.Count;

            return stat;
        }
    }
}