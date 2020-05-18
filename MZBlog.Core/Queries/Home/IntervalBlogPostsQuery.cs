using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using MZBlog.Core.Entities;
using MZBlog.Core.Enums;
using System;
using System.Collections.Generic;

namespace MZBlog.Core.Queries.Home
{
    public class IntervalBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

    public class IntervalBlogPostsQuery : IRequest<IntervalBlogPostsViewModel>
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
        public PublishStatus Status { get; set; } = PublishStatus.Published;
    }

    public class IntervalBlogPostsViewProjection : RequestHandler<IntervalBlogPostsQuery, IntervalBlogPostsViewModel>
    {
        private readonly SqliteConnection _conn;

        public IntervalBlogPostsViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override IntervalBlogPostsViewModel Handle(IntervalBlogPostsQuery request)
        {
            var list = _conn.Query<BlogPost>(@"SELECT * FROM BlogPost
                                               WHERE [Status]=@Status
                                                  AND PublishUTC<@utcNow
                                                  AND PublishUTC>@FromDate
                                                  AND PublishUTC<@ToDate
                                                ORDER BY PublishUTC DESC", new
            {
                utcNow = DateTime.UtcNow,
                request.Status,
                request.FromDate,
                request.ToDate
            });

            return new IntervalBlogPostsViewModel
            {
                Posts = list,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };
        }
    }
}