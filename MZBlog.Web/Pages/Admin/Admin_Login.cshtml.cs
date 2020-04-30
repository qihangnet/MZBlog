using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Commands.Accounts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MZBlog.Web.Pages.Admin
{
    public class Admin_LoginModel : PageModel
    {
        private readonly IMediator _mediator;

        public Admin_LoginModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        [Required]
        public string Account { get; set; }

        [BindProperty]
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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.Author.Id),
                    new Claim(ClaimTypes.Name, result.Author.DisplayName),
                    new Claim(ClaimTypes.Email, result.Author.Email),
                    new Claim(ClaimTypes.Role, "admin"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                return Redirect("/admin");
            }
            else
            {
                ViewData["error-msg"] = "登录失败";
                return Page();
            }
        }
    }
}