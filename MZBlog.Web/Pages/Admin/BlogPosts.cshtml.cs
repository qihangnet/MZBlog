using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Queries.Admin;
using System.Threading.Tasks;

namespace MZBlog.Web.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class BlogPostsModel : PageModel
    {
        private readonly IMediator _mediator;

        public BlogPostsModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public AllBlogPostsViewModel BlogPosts { get; set; }

        public async Task OnGetAsync()
        {
            var pageNo = 1;
            if (Request.Query.ContainsKey("page"))
            {
                int.TryParse(Request.Query["page"], out pageNo);
            }
            var query = new AllBlogPostsQuery() { Page = pageNo, Take = 10 };
            BlogPosts = await _mediator.Send(query);
        }
    }
}