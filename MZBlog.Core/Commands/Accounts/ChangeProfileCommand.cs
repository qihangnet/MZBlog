using iBoxDB.LocalServer;
using MZBlog.Core.Documents;

namespace MZBlog.Core.Commands.Accounts
{
    public class ChangeProfileCommand
    {
        public string AuthorId { get; set; }

        public string NewEmail { get; set; }

        public string NewDisplayName { get; set; }
    }

    public class ChangeProfileCommandInvoker : ICommandInvoker<ChangeProfileCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public ChangeProfileCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        public CommandResult Execute(ChangeProfileCommand command)
        {
            var author = _db.SelectKey<Author>(DBTableNames.Authors, command.AuthorId);
            if (author == null)
                return new CommandResult("用户信息不存在");
            author.DisplayName = command.NewDisplayName;
            author.Email = command.NewEmail;

            _db.Update(DBTableNames.Authors, author);
            return CommandResult.SuccessResult;
        }
    }
}