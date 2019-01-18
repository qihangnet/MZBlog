using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Extensions;

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
            var list = _conn.Query<BlogPost>("select * from BlogPost where PublishUTC>@FromDate and PublishUTC<@ToDate order by PublishUTC desc",request);

            return new IntervalBlogPostsViewModel
            {
                Posts = list,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };
        }
    }
}