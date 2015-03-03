using iBoxDB.LocalServer;
using MZBlog.Core.Documents;

namespace MZBlog.Core.Commands.Accounts
{
    public class ChangePasswordCommand
    {
        public string AuthorId { get; set; }

        public string NewPassword { get; set; }

        public string NewPasswordConfirm { get; set; }

        public string OldPassword { get; set; }
    }

    public class ChangePasswordCommandInvoker : ICommandInvoker<ChangePasswordCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public ChangePasswordCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        public CommandResult Execute(ChangePasswordCommand command)
        {
            var author = _db.SelectKey<Author>(DBTableNames.Authors, command.AuthorId);
            if (Hasher.GetMd5Hash(command.OldPassword) != author.HashedPassword)
            {
                return new CommandResult("旧密码不正确!");
            }

            author.HashedPassword = Hasher.GetMd5Hash(command.NewPassword);
            _db.Update(DBTableNames.Authors, author);
            return CommandResult.SuccessResult;
        }
    }
}