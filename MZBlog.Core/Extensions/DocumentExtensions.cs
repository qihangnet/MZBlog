namespace MZBlog.Core.Extensions
{
    public static class DocumentExtensions
    {
        public static ObjectId ToObjectId(this string objectIdstringRtprsetntation)
        {
            return new ObjectId(objectIdstringRtprsetntation);
        }
    }
}