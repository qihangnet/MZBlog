using Microsoft.Data.Sqlite;
using MediatR;
using MZBlog.Core.Documents;
using Dapper.Extensions;

namespace MZBlog.Core.Queries.Account
{
    public class AuthorDetailQuery : IRequest<Author>
    {
        public string Id { get; set; }
    }

    public class GetUserDetails : RequestHandler<AuthorDetailQuery, Author>
    {
        private readonly SqliteConnection _conn;

        public GetUserDetails(SqliteConnection conn)
        {
            _conn = conn;
        }

        protected override Author Handle(AuthorDetailQuery request)
        {
            return _conn.Get<Author>(request.Id);
        }
    }
}