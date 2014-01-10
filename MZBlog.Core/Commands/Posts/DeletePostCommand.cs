using MongoDB.Driver.Builders;
using MZBlog.Core.Extensions;

namespace MZBlog.Core.Commands.Posts
{
    public class DeletePostCommand
    {
        public string PostId { get; set; }
    }

    public class DeletePostCommandInvoker : ICommandInvoker<DeletePostCommand, CommandResult>
    {
        private readonly MongoCollections _collections;

        public DeletePostCommandInvoker(MongoCollections collections)
        {
            _collections = collections;
        }

        public CommandResult Execute(DeletePostCommand command)
        {
            _collections.BlogCommentCollection.Remove(Query.EQ("PostId", command.PostId.ToObjectId()));
            _collections.BlogPostCollection.Remove(Query.EQ("_id", command.PostId.ToObjectId()));

            return CommandResult.SuccessResult;
        }
    }
}