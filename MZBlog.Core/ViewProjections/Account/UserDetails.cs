using iBoxDB.LocalServer;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Account
{
    public class GetUserDetails : IViewProjection<string, Author>
    {
        private readonly DB.AutoBox _db;

        public GetUserDetails(DB.AutoBox db)
        {
            _db = db;
        }

        public Author Project(string input)
        {
            return _db.SelectKey<Author>(DBTableNames.Authors, input);
        }
    }
}