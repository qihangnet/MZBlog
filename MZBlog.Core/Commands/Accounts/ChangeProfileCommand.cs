using MongoDB.Bson;

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
        private readonly MongoCollections _collections;

        public ChangeProfileCommandInvoker(MongoCollections collections)
        {
            _collections = collections;
        }

        public CommandResult Execute(ChangeProfileCommand command)
        {
            var authorCol = _collections.AuthorCollection;
            var author = authorCol.FindOneById(new ObjectId(command.AuthorId));

            author.DisplayName = command.NewDisplayName;
            author.Email = command.NewEmail;

            authorCol.Save(author);

            return CommandResult.SuccessResult;
        }
    }
}