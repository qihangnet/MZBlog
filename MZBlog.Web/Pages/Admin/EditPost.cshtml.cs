using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Commands.Posts;
using MZBlog.Core.Queries.Account;
using MZBlog.Core.Queries.Admin;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MZBlog.Web.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class EditPostModel : PageModel
    {
        private readonly IMediator _mediator;

        public EditPostModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty(SupportsGet = true)]
        [Required]
        public string Id { get; set; }

        [BindProperty]
        [Required]
        public string Title { get; set; }

        [BindProperty]
        [Required]
        public string TitleSlug { get; set; }

        [BindProperty]
        public string MarkDown { get; set; }

        [BindProperty]
        public string Tags { get; set; }

        [BindProperty]
        [Required]
        public DateTime PubDate { get; set; }

        [BindProperty]
        public bool Published { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Id))
            {
                return NotFound();
            }
            var postQueryById = new BlogPostEditQuery { PostId = Id };
            var data = await _mediator.Send(postQueryById);
            data.BlogPost.Adapt(this);
            this.Tags = string.Join(",", data.BlogPost.Tags);
            this.Published = data.BlogPost.IsPublished;
            this.PubDate = data.BlogPost.PublishUTC;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var author = await _mediator.Send(new AuthorDetailQuery { Id = this.User.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier).Value });

            var cmd = this.Adapt<EditPostCommand>();
            cmd.PostId = Id;

            var result = await _mediator.Send(cmd);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.GetErrors().First());
                return Page();
            }

            return Redirect("/admin/posts");
        }
    }
}