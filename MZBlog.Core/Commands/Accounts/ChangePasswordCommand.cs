using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;

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
        private readonly DB.AutoBox _db;

        public ChangePasswordCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        protected override CommandResult Handle(ChangePasswordCommand cmd)
        {
            var author = _db.SelectKey<Author>(DBTableNames.Authors, cmd.AuthorId);
            if (Hasher.GetMd5Hash(cmd.OldPassword) != author.HashedPassword)
            {
                return new CommandResult("旧密码不正确!");
            }

            author.HashedPassword = Hasher.GetMd5Hash(cmd.NewPassword);
            _db.Update(DBTableNames.Authors, author);
            return CommandResult.SuccessResult;
        }
    }
}