using MongoDB.Driver.Builders;
using MZBlog.Core.Extensions;

namespace MZBlog.Core.Commands.Posts
{
    public class DeleteCommentCommand
    {
        public string CommentId { get; set; }
    }

    public class DeleteCommentCommandInvoker : ICommandInvoker<DeleteCommentCommand, CommandResult>
    {
        private readonly MongoCollections _collections;

        public DeleteCommentCommandInvoker(MongoCollections collections)
        {
            _collections = collections;
        }

        public CommandResult Execute(DeleteCommentCommand command)
        {
            _collections.BlogCommentCollection.Remove(Query.EQ("_id", command.CommentId.ToObjectId()));

            return CommandResult.SuccessResult;
        }
    }
}