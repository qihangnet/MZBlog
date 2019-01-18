using Dapper.Extensions;

namespace MZBlog.Core.Entities
{
    public class BlogPostTags
    {
        [ExplicitKey]
        public string BlogPostId { get; set; }

        [ExplicitKey]
        public string TagSlug { get; set; }
    }
}