using iBoxDB.LocalServer;
using MZBlog.Core.Documents;
using System.Linq;

namespace MZBlog.Core.Commands.Posts
{
    public class DeletePostCommand
    {
        public string PostId { get; set; }
    }

    public class DeletePostCommandInvoker : ICommandInvoker<DeletePostCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public DeletePostCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        public CommandResult Execute(DeletePostCommand command)
        {
            var comments = _db.Select<BlogComment>("from " + DBTableNames.BlogComments + " where PostId==?", command.PostId);

            if (comments.Count() > 0)
            {
                var commentKeys = comments.Select(s => s.Id).ToArray();
                _db.Delete(DBTableNames.BlogComments, commentKeys);
            }
            _db.Delete(DBTableNames.BlogPosts, command.PostId);
            return CommandResult.SuccessResult;
        }
    }
}