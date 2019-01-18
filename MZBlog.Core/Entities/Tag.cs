using Dapper.Extensions;

namespace MZBlog.Core.Entities
{
    public class Tag
    {
        [ExplicitKey]
        public string Slug { get; set; }

        public string Name { get; set; }

        public int PostCount { get; set; }
    }
}