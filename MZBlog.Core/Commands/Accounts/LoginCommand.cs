using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MZBlog.Core.Commands.Accounts
{
    public class LoginCommand : IRequest<LoginCommandResult>
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class LoginCommandResult : CommandResult
    {
        public LoginCommandResult()
            : base()
        { }

        public LoginCommandResult(string errorMessage)
            : base(errorMessage)
        { }

        public Author Author { get; set; }
    }

    public class LoginCommandInvoker : RequestHandler<LoginCommand, LoginCommandResult>
    {
        private readonly DB.AutoBox _db;

        public LoginCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        protected override LoginCommandResult Handle(LoginCommand cmd)
        {
            var hashedPassword = Hasher.GetMd5Hash(cmd.Password);
            if (_db.SelectCount("from " + DBTableNames.Authors) == 0)
            {
                _db.Insert(DBTableNames.Authors, new Author
                {
                    Email = "mz@bl.og",
                    DisplayName = "mzblog",
                    Roles = new[] { "admin" },
                    HashedPassword = Hasher.GetMd5Hash("mzblog")
                });
            }
            var author = from u in _db.Select<Author>("from " + DBTableNames.Authors)
                         where u.Email == cmd.Email && u.HashedPassword == hashedPassword
                         select u;

            if (author.Any())
                return new LoginCommandResult() { Author = author.FirstOrDefault() };

            return new LoginCommandResult(errorMessage: "用户名或密码不正确");
        }
    }
}