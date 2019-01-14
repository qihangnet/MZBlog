using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;
using System.Linq;

namespace MZBlog.Core.Commands.Posts
{
    public class DeletePostCommand : IRequest<CommandResult>
    {
        public string PostId { get; set; }
    }

    public class DeletePostCommandInvoker : RequestHandler<DeletePostCommand, CommandResult>
    {
        private readonly DB.AutoBox _db;

        public DeletePostCommandInvoker(DB.AutoBox db)
        {
            _db = db;
        }

        protected override CommandResult Handle(DeletePostCommand cmd)
        {
            var comments = _db.Select<BlogComment>("from " + DBTableNames.BlogComments + " where PostId==?", cmd.PostId);

            if (comments.Count() > 0)
            {
                var commentKeys = comments.Select(s => s.Id).ToArray();
                _db.Delete(DBTableNames.BlogComments, commentKeys);
            }
            _db.Delete(DBTableNames.BlogPosts, cmd.PostId);
            return CommandResult.SuccessResult;
        }
    }
}