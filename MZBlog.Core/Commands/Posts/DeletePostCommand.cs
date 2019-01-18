using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using System.Linq;
using Dapper;
using Dapper.Extensions;

namespace MZBlog.Core.Commands.Posts
{
    public class DeletePostCommand : IRequest<CommandResult>
    {
        public string PostId { get; set; }
    }

    public class DeletePostCommandInvoker : RequestHandler<DeletePostCommand, CommandResult>
    {
        private readonly SqliteConnection _conn;

        public DeletePostCommandInvoker(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override CommandResult Handle(DeletePostCommand cmd)
        {
            _conn.Open();
            using (var tran = _conn.BeginTransaction())
            {
                var comments = _conn.Query<string>("select Id from BlogComment where PostId=@PostId", new { cmd.PostId }, tran);

                if (comments.Count() > 0)
                {
                    var commentKeys = comments.ToArray();
                    _conn.Execute("delete from BlogComments where Id in @commentKeys", new { commentKeys }, tran);
                }
                _conn.Execute("delete from BlogPost where Id=@PostId", new { cmd.PostId }, tran);
                tran.Commit();
            }
            _conn.Close();

            return CommandResult.SuccessResult;
        }
    }
}