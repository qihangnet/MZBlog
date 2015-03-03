using System;

namespace MZBlog.Core.Documents
{
    public class SpamHash
    {
        public string Id { get; set; }

        public string PostKey { get; set; }

        public string Hash { get; set; }

        public bool Pass { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}