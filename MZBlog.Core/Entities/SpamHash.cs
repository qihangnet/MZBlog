using Dapper.Extensions;
using System;

namespace MZBlog.Core.Entities
{
    public class SpamHash
    {
        [ExplicitKey]
        public string Id { get; set; }

        public string PostKey { get; set; }

        public string Hash { get; set; }

        public bool Pass { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}