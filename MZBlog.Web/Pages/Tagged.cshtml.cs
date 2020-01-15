using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Extensions;
using MZBlog.Core.Queries.Home;

namespace MZBlog.Web.Pages
{
    public class TaggedModel : PageModel
    {
        private readonly IMediator _mediator;

        public TaggedModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty(SupportsGet = true)]
        public string Tag { get; set; }

        public TaggedBlogPostsViewModel Data { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Data = await _mediator.Send(new TaggedBlogPostsQuery() { Tag = Tag }).ConfigureAwait(false);
            if (Data.Tag.IsNullOrWhitespace())
            {
                return NotFound();
            }
            return Page();
        }
    }
}