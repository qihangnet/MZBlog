using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class IntervalBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

    public class IntervalBlogPostsBindingModel:IRequest<IntervalBlogPostsViewModel>
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

    public class IntervalBlogPostsViewProjection : RequestHandler<IntervalBlogPostsBindingModel, IntervalBlogPostsViewModel>
    {
        private readonly DB.AutoBox _db;

        public IntervalBlogPostsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        protected override IntervalBlogPostsViewModel Handle(IntervalBlogPostsBindingModel request)
        {
            var posts = from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                        where p.IsPublished && p.PubDate < request.ToDate && p.PubDate > request.FromDate
                        orderby p.PubDate descending
                        select p;

            return new IntervalBlogPostsViewModel
            {
                Posts = posts,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };
        }
    }
}