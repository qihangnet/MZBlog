using iBoxDB.LocalServer;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AuthorProfileViewModel
    {
        public string DisplayName { get; set; }

        public string Email { get; set; }
    }

    public class AuthorProfileViewProjection : IViewProjection<string, AuthorProfileViewModel>
    {
        private readonly DB.AutoBox _db;

        public AuthorProfileViewProjection(DB.AutoBox db)
        {
            _db = db;
        }

        public AuthorProfileViewModel Project(string input)
        {
            var author = _db.SelectKey<Author>(DBTableNames.Authors, input);
            if (author == null)
                return null;
            return new AuthorProfileViewModel
            {
                DisplayName = author.DisplayName,
                Email = author.Email
            };
        }
    }
}