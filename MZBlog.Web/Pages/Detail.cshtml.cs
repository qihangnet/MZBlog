using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Queries.Home;

namespace MZBlog.Web.Pages
{
    public class DetailModel : PageModel
    {
        private readonly IMediator _mediator;

        public DetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty(SupportsGet = true)]
        public int Year { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Month { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        public BlogPostDetailsViewModel Detail { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var query = new BlogPostDetailsQuery { Permalink = Slug };
            Detail = await _mediator.Send(query).ConfigureAwait(false);
            if (Detail == null || Detail.BlogPost==null)
            {
                return this.NotFound();
            }
            return Page();
        }
    }
}