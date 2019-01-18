using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System;

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
            var list = _conn.Query<BlogPost>($"select Title from BlogPost where PublishUTC<@utcNow order by PublishUTC desc limit {request.Page}", new { utcNow = DateTime.UtcNow });
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