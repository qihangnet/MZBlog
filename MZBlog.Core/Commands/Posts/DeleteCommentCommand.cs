using iBoxDB.LocalServer;

namespace MZBlog.Core.Commands.Posts
{
    public class DeleteCommentCommand
    {
        public string CommentId { get; set; }
    }

    public class DeleteCommentCommandInvoker : ICommandInvoker<DeleteCommentCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public DeleteCommentCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        public CommandResult Execute(DeleteCommentCommand command)
        {
            _db.Delete(DBTableNames.BlogComments, command.CommentId);

            return CommandResult.SuccessResult;
        }
    }
}