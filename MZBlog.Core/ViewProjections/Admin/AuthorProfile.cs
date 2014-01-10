using MongoDB.Driver.Linq;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AuthorProfileViewModel
    {
        public string DisplayName { get; set; }

        public string Email { get; set; }
    }

    public class AuthorProfileViewProjection : IViewProjection<string, AuthorProfileViewModel>
    {
        private readonly MongoCollections _collections;

        public AuthorProfileViewProjection(MongoCollections collections)
        {
            _collections = collections;
        }

        public AuthorProfileViewModel Project(string input)
        {
            var author = _collections.AuthorCollection.AsQueryable().FirstOrDefault(w=>w.Id==input);

            return new AuthorProfileViewModel
            {
                DisplayName = author.DisplayName,
                Email = author.Email
            };
        }
    }
}
