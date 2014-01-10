using System.Linq;

namespace MZBlog.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, int page = 1, int pageSize = 10)
        {
            return queryable.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}