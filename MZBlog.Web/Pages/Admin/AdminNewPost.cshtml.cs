using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MZBlog.Core.Commands.Posts;
using MZBlog.Core.Queries.Account;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MZBlog.Web.Pages.Admin
{
    public class AdminNewPostModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AdminNewPostModel> _logger;

        public AdminNewPostModel(IMediator mediator, ILogger<AdminNewPostModel> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public void OnGet()
        {
        }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var author = await _mediator.Send(new AuthorDetailQuery { Id = this.User.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier).Value });

            var cmd = this.Adapt<NewPostCommand>();
            cmd.Author = author;

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