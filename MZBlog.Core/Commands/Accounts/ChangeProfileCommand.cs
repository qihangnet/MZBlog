using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using Dapper;
using Dapper.Extensions;

namespace MZBlog.Core.Commands.Accounts
{
    public class ChangeProfileCommand : IRequest<CommandResult>
    {
        public string AuthorId { get; set; }

        public string NewEmail { get; set; }

        public string NewDisplayName { get; set; }
    }

    public class ChangeProfileCommandInvoker : RequestHandler<ChangeProfileCommand, CommandResult>
    {
        private readonly SqliteConnection _conn;

        public ChangeProfileCommandInvoker(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override CommandResult Handle(ChangeProfileCommand cmd)
        {
            var author = _conn.Get<Author>(cmd.AuthorId);
            if (author == null)
                return new CommandResult("用户信息不存在");
            author.DisplayName = cmd.NewDisplayName;
            author.Email = cmd.NewEmail;

            _conn.Update(author);
            return CommandResult.SuccessResult;
        }
    }
}