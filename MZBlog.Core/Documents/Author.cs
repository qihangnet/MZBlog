using System;
using Dapper.Extensions;

namespace MZBlog.Core.Documents
{
    public class Author
    {
        public Author()
        {
            Id = ObjectId.NewId();
        }

        [ExplicitKey]
        public string Id { get; set; }

        public string HashedPassword { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public DateTime CreatedUTC { get; set; }
    }
}