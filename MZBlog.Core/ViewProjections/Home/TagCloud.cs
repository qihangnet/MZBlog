using MongoDB.Bson;
using MongoDB.Driver.Builders;
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
        private readonly MongoCollections _collections;

        public TagCloudViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public TagCloudViewModel Project(TagCloudBindingModel input)
        {
            var blogPosts = _collections.BlogPostCollection;
            if (blogPosts.Count() == 0)
                return new TagCloudViewModel { Tags = new Dictionary<Tag, int>() };
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

            var mr = blogPosts.MapReduce(query, map, reduce);

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
                           .Where(k => k.counter >= input.Threshold)
                           .OrderByDescending(k => k.counter)
                           .Take(input.Take)
                           .ToDictionary(k => k.tag, v => v.counter);

            return new TagCloudViewModel
            {
                Tags = result
            };
        }
    }
}