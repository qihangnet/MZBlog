using MZBlog.Core;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Web.Security;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;

namespace MZBlog.Web.Modules
{
    public class AccountModule : NancyModule
    {
        private readonly ICommandInvokerFactory _commandInvoker;

        public AccountModule(ICommandInvokerFactory commandInvoker)
        {
            _commandInvoker = commandInvoker;

            Get["/mz-login"] = _ => ShowLoginPage();
            Post["/mz-login"] = _ => LoginUser(this.BindAndValidate<LoginCommand>());

            Get["/mz-logout"] = _ => Logout();
        }

        public Negotiator Logout()
        {
            return View["LogoutPage"].WithCookie(FormsAuthentication.CreateLogoutCookie());
        }

        public Negotiator ShowLoginPage()
        {
            ViewBag.ReturnUrl = Request.Query.returnUrl;
            return View["LoginPage"];
        }

        public dynamic LoginUser(LoginCommand loginCommand)
        {
            if (!ModelValidationResult.IsValid)
            {
                return View["LoginPage", new[] { "请正确输入Email和密码" }];
            }

            var commandResult = _commandInvoker.Handle<LoginCommand, LoginCommandResult>(loginCommand);

            if (commandResult.Success)
            {
                var cookie = FormsAuthentication.CreateAuthCookie(commandResult.Author.Id);
                var response = Context.GetRedirect(loginCommand.ReturnUrl ?? "/mz-admin");
                response.WithCookie(cookie);
                return response;
            }

            return View["LoginPage", commandResult.GetErrors()];
        }
    }
}