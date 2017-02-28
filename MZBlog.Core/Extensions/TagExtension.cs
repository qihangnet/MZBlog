using MZBlog.Core.Documents;

namespace MZBlog.Core.Extensions
{
    public static class TagExtension
    {
        private static IViewProjectionFactory _viewFac;

        public static void SetupViewProjectionFactory(IViewProjectionFactory fac)
        {
            _viewFac = fac;
        }

        public static Tag AsTag(this string slug)
        {
            return _viewFac.Get<string, Tag>(slug);
        }
    }
}