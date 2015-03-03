using iBoxDB.LocalServer;
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

    public class IntervalBlogPostsBindingModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

    public class IntervalBlogPostsViewProjection : IViewProjection<IntervalBlogPostsBindingModel, IntervalBlogPostsViewModel>
    {
        private readonly DB.AutoBox _db;

        public IntervalBlogPostsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public IntervalBlogPostsViewModel Project(IntervalBlogPostsBindingModel input)
        {
            var posts = from p in _db.Select<BlogPost>("from " + DBTableNames.BlogPosts)
                        where p.IsPublished && p.PubDate < input.ToDate && p.PubDate > input.FromDate
                        orderby p.PubDate descending
                        select p;

            return new IntervalBlogPostsViewModel
            {
                Posts = posts,
                FromDate = input.FromDate,
                ToDate = input.ToDate
            };
        }
    }
}