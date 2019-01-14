using iBoxDB.LocalServer;
using MediatR;

namespace MZBlog.Core.Commands.Posts
{
    public class DeleteCommentCommand : IRequest<CommandResult>
    {
        public string CommentId { get; set; }
    }

    public class DeleteCommentCommandInvoker : RequestHandler<DeleteCommentCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public DeleteCommentCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        protected override CommandResult Handle(DeleteCommentCommand cmd)
        {
            _db.Delete(DBTableNames.BlogComments, cmd.CommentId);

            return CommandResult.SuccessResult;
        }
    }
}