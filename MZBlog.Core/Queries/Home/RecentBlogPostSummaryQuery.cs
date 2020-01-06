using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using MZBlog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.Queries.Home
{
    public class RecentBlogPostSummaryViewModel
    {
        public IEnumerable<BlogPostSummary> BlogPostsSummaries { get; set; }
    }

    public class BlogPostSummary
    {
        public string Title { get; set; }

        public string Link { get; set; }
    }

    public class RecentBlogPostSummaryQuery : IRequest<RecentBlogPostSummaryViewModel>
    {
        public int Page { get; set; }
    }

    public class RecentBlogPostSummaryViewProjection : RequestHandler<RecentBlogPostSummaryQuery, RecentBlogPostSummaryViewModel>
    {
        private readonly SqliteConnection _conn;

        public RecentBlogPostSummaryViewProjection(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override RecentBlogPostSummaryViewModel Handle(RecentBlogPostSummaryQuery request)
        {
            var list = _conn.Query<BlogPost>($@"SELECT Title FROM BlogPost
                                                WHERE [Status]=@status
                                                    AND PublishUTC<@utcNow
                                                ORDER BY PublishUTC desc
                                                LIMIT {request.Page}", new { utcNow = DateTime.UtcNow, status = PublishStatus.Published });
            var titles = (from p in list
                          select new BlogPostSummary
                          {
                              Title = p.Title,
                              Link = p.GetLink()
                          }
                          )
                .Take(request.Page)
                .ToList()
                .AsReadOnly();

            return new RecentBlogPostSummaryViewModel { BlogPostsSummaries = titles };
        }
    }
}