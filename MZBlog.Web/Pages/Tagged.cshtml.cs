using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Queries.Home;
using System.Threading.Tasks;

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

        public async Task OnGetAsync()
        {
            Data = await _mediator.Send(new TaggedBlogPostsQuery() { Tag = Tag }).ConfigureAwait(false);
        }
    }
}