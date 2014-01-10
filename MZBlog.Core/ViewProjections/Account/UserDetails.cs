using MongoDB.Driver.Linq;
using MZBlog.Core.Documents;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Account
{
    public class GetUserDetails : IViewProjection<string, Author>
    {
        private readonly MongoCollections _collections;

        public GetUserDetails(MongoCollections collections)
        {
            _collections = collections;
        }

        public Author Project(string input)
        {
            return _collections.AuthorCollection.AsQueryable()
                            .FirstOrDefault(a => a.Id == input);
        }
    }
}