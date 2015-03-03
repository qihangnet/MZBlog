using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AllBlogCommentsBindingModel
    {
        public AllBlogCommentsBindingModel()
        {
            Page = 1;
            Take = 20;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class AllBlogCommentsViewModel
    {
        public IEnumerable<BlogComment> Comments { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPrevPage
        {
            get
            {
                return Page > 1;
            }
        }

        public int Page { get; set; }
    }

    public class BlogCommentsViewProjection : IViewProjection<AllBlogCommentsBindingModel, AllBlogCommentsViewModel>
    {
        private readonly DB.AutoBox _db;

        public BlogCommentsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public AllBlogCommentsViewModel Project(AllBlogCommentsBindingModel input)
        {
            var skip = (input.Page - 1) * input.Take;

            var comments = _db.Select<BlogComment>("from " + DBTableNames.BlogComments + " order by CreatedTime desc limit " + skip + "," + input.Take + 1)
                .ToList().AsReadOnly();

            var pagedComments = comments.Take(input.Take);
            var hasNextPage = comments.Count > input.Take;

            return new AllBlogCommentsViewModel
            {
                Comments = pagedComments,
                Page = input.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}