using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Commands.Accounts;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MZBlog.Web.Pages
{
    public class Admin_LoginModel : PageModel
    {
        private readonly IMediator _mediator;

        public Admin_LoginModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty(SupportsGet = true)]
        [Required]
        public string Account { get; set; }

        [BindProperty(SupportsGet = true)]
        [Required]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["error-msg"] = "请填写账号密码";
                return Page();
            }

            var cmd = new LoginCommand
            {
                Email = Account,
                Password = Password
            };

            var result = await _mediator.Send(cmd).ConfigureAwait(false);
            if (result.Success)
            {
                return Redirect("/");
            }
            else
            {
                ViewData["error-msg"] = "登录失败";
                return Page();
            }
        }
    }
}