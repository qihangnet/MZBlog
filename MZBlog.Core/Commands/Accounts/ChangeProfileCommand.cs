using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;

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
        private readonly DB.AutoBox _db;

        public ChangeProfileCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        protected override CommandResult Handle(ChangeProfileCommand cmd)
        {
            var author = _db.SelectKey<Author>(DBTableNames.Authors, cmd.AuthorId);
            if (author == null)
                return new CommandResult("用户信息不存在");
            author.DisplayName = cmd.NewDisplayName;
            author.Email = cmd.NewEmail;

            _db.Update(DBTableNames.Authors, author);
            return CommandResult.SuccessResult;
        }
    }
}