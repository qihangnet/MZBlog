using MongoDB.Bson;

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
        private readonly MongoCollections _collections;

        public ChangePasswordCommandInvoker(MongoCollections collections)
        {
            _collections = collections;
        }

        public CommandResult Execute(ChangePasswordCommand command)
        {
            var authorCol = _collections.AuthorCollection;
            var author = authorCol.FindOneById(new ObjectId(command.AuthorId));

            if (Hasher.GetMd5Hash(command.OldPassword) != author.HashedPassword)
            {
                return new CommandResult("旧密码不正确!");
            }

            author.HashedPassword = Hasher.GetMd5Hash(command.NewPassword);
            authorCol.Save(author);
            return CommandResult.SuccessResult;
        }
    }
}