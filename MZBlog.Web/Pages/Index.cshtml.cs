using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Queries.Home;
using System.Threading.Tasks;

namespace MZBlog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public RecentBlogPostsViewModel RecentBlogPosts { get; set; }

        public async Task OnGetAsync()
        {
            var pageNo = 1;
            if (Request.Query.ContainsKey("page"))
            {
                int.TryParse(Request.Query["page"], out pageNo);
            }
            var query = new RecentBlogPostsQuery() { Page = pageNo, Take = 10 };
            RecentBlogPosts = await _mediator.Send(query);
        }
    }
}