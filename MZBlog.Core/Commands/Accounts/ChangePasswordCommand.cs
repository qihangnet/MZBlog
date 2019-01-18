using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Entities;
using Dapper;
using Dapper.Extensions;

namespace MZBlog.Core.Commands.Accounts
{
    public class ChangePasswordCommand : IRequest<CommandResult>
    {
        public string AuthorId { get; set; }

        public string NewPassword { get; set; }

        public string NewPasswordConfirm { get; set; }

        public string OldPassword { get; set; }
    }

    public class ChangePasswordCommandInvoker : RequestHandler<ChangePasswordCommand, CommandResult>
    {
        private readonly SqliteConnection _conn;

        public ChangePasswordCommandInvoker(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override CommandResult Handle(ChangePasswordCommand cmd)
        {
            var author = _conn.Get<Author>(cmd.AuthorId);
            if (Hasher.GetMd5Hash(cmd.OldPassword) != author.HashedPassword)
            {
                return new CommandResult("旧密码不正确!");
            }

            author.HashedPassword = Hasher.GetMd5Hash(cmd.NewPassword);
            _conn.Update(author);
            return CommandResult.SuccessResult;
        }
    }
}