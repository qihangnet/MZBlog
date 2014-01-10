using MongoDB.Driver.Linq;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
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
        private readonly MongoCollections _collections;

        public BlogCommentsViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public AllBlogCommentsViewModel Project(AllBlogCommentsBindingModel input)
        {
            var comments = _collections.BlogCommentCollection
                        .AsQueryable()
                        .OrderByDescending(b => b.CreatedTime)
                        .TakePage(input.Page, pageSize: input.Take + 1)
                        .ToList();
            var pagedComments = comments.Take(input.Take);
            var hasNextPage = comments.Count > input.Take;

            return new AllBlogCommentsViewModel
            {
                Comments = comments,
                Page = input.Page,
                HasNextPage = hasNextPage
            };
        }
    }
}