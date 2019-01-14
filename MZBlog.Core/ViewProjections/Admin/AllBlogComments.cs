using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AllBlogCommentsBindingModel:IRequest<AllBlogCommentsViewModel>
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

    public class BlogCommentsViewProjection : RequestHandler<AllBlogCommentsBindingModel, AllBlogCommentsViewModel>
    {
        private readonly DB.AutoBox _db;

        public BlogCommentsViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        protected override AllBlogCommentsViewModel Handle(AllBlogCommentsBindingModel request)
        {
            var skip = (request.Page - 1) * request.Take;

            var comments = _db.Select<BlogComment>("from " + DBTableNames.BlogComments + " order by CreatedTime desc limit " + skip + "," + request.Take + 1)
                .ToList().AsReadOnly();

            var pagedComments = comments.Take(request.Take);
            var hasNextPage = comments.Count > request.Take;

            return new AllBlogCommentsViewModel
            {
                Comments = pagedComments,
                Page = request.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}