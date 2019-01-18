using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dapper;
using Dapper.Extensions;

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
        private readonly SqliteConnection _conn;

        public LoginCommandInvoker(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override LoginCommandResult Handle(LoginCommand cmd)
        {
            var hashedPassword = Hasher.GetMd5Hash(cmd.Password);
            if (_conn.ExecuteScalar<int>("select count(1) from Author") == 0)
            {
                _conn.Insert(new Author
                {
                    Email = "mz@bl.og",
                    DisplayName = "mzblog",
                    HashedPassword = Hasher.GetMd5Hash("mzblog")
                });
            }
            var author = _conn.QueryFirstOrDefault<Author>("select * from Author where Email=@email and HashedPassword=@hashedPassword", new { email = cmd.Email, hashedPassword });

            if (author != null)
                return new LoginCommandResult() { Author = author };

            return new LoginCommandResult(errorMessage: "用户名或密码不正确");
        }
    }
}