using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.ViewProjections.Home;

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


        public void OnGet()
        {
            var pageNo = 1;
            if (Request.Query.ContainsKey("page"))
            {
                int.TryParse(Request.Query["page"], out pageNo);
            }
            var query = new RecentBlogPostsQuery() { Page = pageNo, Take = 10 };
            RecentBlogPosts = _mediator.Send(query).Result;
        }
    }
}
