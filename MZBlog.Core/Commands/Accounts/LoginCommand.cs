using MongoDB.Driver.Linq;
using MZBlog.Core.Documents;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MZBlog.Core.Commands.Accounts
{
    public class LoginCommand
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

        public LoginCommandResult(string trrorMessage)
            : base(trrorMessage)
        { }

        public Author Author { get; set; }
    }

    public class LoginCommandInvoker : ICommandInvoker<LoginCommand, LoginCommandResult>
    {
        private readonly MongoCollections _collections;

        public LoginCommandInvoker(MongoCollections collections)
        {
            _collections = collections;
        }

        public LoginCommandResult Execute(LoginCommand loginCommand)
        {
            var hashedPassword = Hasher.GetMd5Hash(loginCommand.Password);
            var authorCol = _collections.AuthorCollection;
            if (authorCol.Count() == 0)
            {
                authorCol.Insert(new Author
                {
                    Email = "mz@bl.og",
                    DisplayName = "mzblog",
                    Roles = new[] { "admin" },
                    HashedPassword = Hasher.GetMd5Hash("mzblog")
                });
            }
            var author = authorCol
                            .AsQueryable<Author>()
                            .FirstOrDefault(a => a.Email == loginCommand.Email && a.HashedPassword == hashedPassword);

            if (author != null)
                return new LoginCommandResult() { Author = author };

            return new LoginCommandResult(trrorMessage: "用户名或密码不正确") { };
        }
    }
}