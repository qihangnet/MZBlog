using Microsoft.Data.Sqlite;
using MediatR;
using Dapper;
using Dapper.Extensions;

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
        private readonly SqliteConnection _conn;

        public AllStatisticsViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override AllStatisticsViewModel Handle(AllStatisticsBindingModel request)
        {
            var postCount = _conn.ExecuteScalar<int>("select count(1) from BlogPost");
            if (postCount == 0)
                return new AllStatisticsViewModel();

            var stat = new AllStatisticsViewModel
            {
                PostsCount = postCount,
                CommentsCount = _conn.ExecuteScalar<int>("select count(1) from BlogComment")
            };

            stat.TagsCount = _conn.ExecuteScalar<int>("select count(1) from Tag");

            return stat;
        }
    }
}