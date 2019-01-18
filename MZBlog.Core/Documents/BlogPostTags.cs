using Dapper.Extensions;

namespace MZBlog.Core.Documents
{
    public class BlogPostTags
    {
        [ExplicitKey]
        public string BlogPostId { get; set; }

        [ExplicitKey]
        public string TagSlug { get; set; }
    }
}