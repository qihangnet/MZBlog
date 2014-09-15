using System.Linq;

namespace MZBlog.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, int page = 1, int pageSize = 10)
        {
            var countSkip = (page - 1) * pageSize - 1;
            if (countSkip < 0)
            {
                countSkip = 0;
            }
            return queryable.Skip(countSkip).Take(pageSize);
        }
    }
}