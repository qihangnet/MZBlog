using iBoxDB.LocalServer;
using MediatR;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Account
{
    public class AuthorDetailQuery : IRequest<Author>
    {
        public string Id { get; set; }
    }

    public class GetUserDetails : RequestHandler<AuthorDetailQuery, Author>
    {
        private readonly DB.AutoBox _db;

        public GetUserDetails(DB.AutoBox db)
        {
            _db = db;
        }

        protected override Author Handle(AuthorDetailQuery request)
        {
            return _db.SelectKey<Author>(DBTableNames.Authors, request.Id);
        }
    }
}