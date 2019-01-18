using MediatR;
using Microsoft.Data.Sqlite;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System;
using Microsoft.Extensions.Logging;

namespace MZBlog.Core.ViewProjections.Home
{
    public class RecentBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public int Page { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPrevPage
        {
            get
            {
                return Page > 1;
            }
        }
    }

    public class RecentBlogPostsQuery : IRequest<RecentBlogPostsViewModel>
    {
        public RecentBlogPostsQuery()
        {
            Page = 1;
            Take = 20;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class RecentBlogPostsQueryHandler : RequestHandler<RecentBlogPostsQuery, RecentBlogPostsViewModel>
    {
        private readonly SqliteConnection _conn;
        private readonly ILogger<RecentBlogPostsQueryHandler> _logger;

        public RecentBlogPostsQueryHandler(SqliteConnection conn, ILogger<RecentBlogPostsQueryHandler> logger)
        {
            _conn = conn;
            _logger = logger;
        }

        protected override RecentBlogPostsViewModel Handle(RecentBlogPostsQuery request)
        {
            var skip = (request.Page - 1) * request.Take;
            var sql = $"SELECT * FROM BlogPost WHERE PublishUTC<@utcNow ORDER BY PublishUTC DESC LIMIT {request.Take + 1} OFFSET {skip}";
            var list = _conn.Query<BlogPost>(sql, new { utcNow = DateTime.UtcNow });
            foreach (var item in list)
            {
                var tags = _conn.Query<string>("SELECT t.Name FROM BlogPostTags p INNER JOIN Tag t ON t.Slug=p.TagSlug WHERE p.BlogPostId=@Id", new { item.Id });
                item.Tags=tags;
            }

            var pagedPosts = list.Take(request.Take);
            var hasNextPage = list.Count() > request.Take;

            return new RecentBlogPostsViewModel
            {
                Posts = pagedPosts,
                Page = request.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}