using MediatR;
using MZBlog.Core.Entities;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using Dapper.Extensions;

namespace MZBlog.Core.Queries.Home
{
    public class TaggedBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public string Tag { get; set; }
    }

    public class TaggedBlogPostsQuery : IRequest<TaggedBlogPostsViewModel>
    {
        public string Tag { get; set; }
    }

    public class TaggedBlogPostsViewProjection : RequestHandler<TaggedBlogPostsQuery, TaggedBlogPostsViewModel>
    {
        private readonly SqliteConnection _conn;

        public TaggedBlogPostsViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override TaggedBlogPostsViewModel Handle(TaggedBlogPostsQuery request)
        {
            var list = _conn.Query<BlogPost>(@"SELECT * FROM BlogPost p 
                                               INNER JOIN BlogPostTags t ON p.Id=t.BlogPostId 
                                               WHERE [Status]=@status AND t.TagSlug=@Tag", new { request.Tag, status = PublishStatus.Published });
            foreach (var item in list)
            {
                var tags = _conn.Query<string>(@"SELECT t.Name FROM BlogPostTags p 
                                                INNER JOIN Tag t ON t.Slug=p.TagSlug 
                                                WHERE p.BlogPostId=@Id", new { item.Id });
                item.Tags = tags;
            }
            var tagName = _conn.Get<Tag>(request.Tag)?.Name;
            return new TaggedBlogPostsViewModel
            {
                Posts = list,
                Tag = tagName
            };
        }
    }
}