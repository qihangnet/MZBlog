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
        private readonly MongoCollections _collections;

        public AllStatisticsViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public AllStatisticsViewModel Project(AllStatisticsBindingModel input)
        {
            var postCol = _collections.BlogPostCollection;
            var commentCol = _collections.BlogCommentCollection;
            if (postCol.Count() == 0)
                return new AllStatisticsViewModel();

            var stat = new AllStatisticsViewModel
            {
                PostsCount = postCol.Count(),
                CommentsCount = commentCol.Count()
            };

            var map = @"function() {
                        	for (var index in this.Tags) {
                                emit(this.Tags[index], { count : 1 });
                            }
                        }";

            var reduce = @"function(key, emits) {
                            total = 0;
                            for (var i in emits) {
                                total += emits[i].count;
                            }
                            return { count : total };
                        }";

            var query = Query<BlogPost>.Where(BlogPost.IsPublished);

            var mr = postCol.MapReduce(new MapReduceArgs { Query = query, MapFunction = map, ReduceFunction = reduce });

            var result = mr.GetResults().Select(el =>
            {
                var tagDoc = el["_id"].AsBsonDocument;

                var tag = new Tag
                {
                    Name = tagDoc["Name"] is BsonNull ? null : tagDoc["Name"].AsString,
                    Slug = tagDoc["Slug"] is BsonNull ? null : tagDoc["Slug"].AsString
                };

                var counter = (int)el["value"]["count"].AsDouble;

                return new { tag, counter };
            })
            .Where(k => k.counter >= input.TagThreshold)
            .OrderByDescending(k => k.counter)
            .ToDictionary(k => k.tag, v => v.counter);
            stat.TagsCount = result.Count;

            return stat;
        }
    }
}