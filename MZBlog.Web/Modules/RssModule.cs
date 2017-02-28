using MZBlog.Core;
using MZBlog.Core.Cache;
using MZBlog.Core.ViewProjections.Home;
using MZBlog.Web.Features;
using MZBlog.Web.Responses;
using System;

namespace MZBlog.Web.Modules
{
    public class RssModule : BaseNancyModule
    {
        private readonly IViewProjectionFactory _viewProjectionFactory;

        private readonly ICache _cache;

        public RssModule(IViewProjectionFactory viewProjectionFactory, ICache cache)
        {
            _viewProjectionFactory = viewProjectionFactory;
            _cache = cache;

            Get["/rss"] = _ => GetRecentPostsRss();
        }

        private dynamic GetRecentPostsRss()
        {
            var cacheKey = "rss";
            var rss = _cache.Get<RssResponse>(cacheKey);
            if (rss == null)
            {
                var recentPosts = _viewProjectionFactory.Get<RecentBlogPostsBindingModel, RecentBlogPostsViewModel>(new RecentBlogPostsBindingModel()
                {
                    Page = 1,
                    Take = 30
                });

                rss = new RssResponse(recentPosts.Posts, Settings.WebsiteName, new Uri(AppConfiguration.Current.SiteUrl));
                _cache.Add(cacheKey, rss, 60 * 5);
            }
            return rss;
        }
    }
}