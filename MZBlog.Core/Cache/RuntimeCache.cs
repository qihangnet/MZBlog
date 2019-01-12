using Microsoft.Extensions.Caching.Memory;
using System;

namespace MZBlog.Core.Cache
{
    public class RuntimeCache : ICache
    {
        private readonly MemoryCache _cache;
        private readonly MemoryCacheEntryOptions _defaultCacheItemPolicy;

        public RuntimeCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _defaultCacheItemPolicy = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(60 * 2) };
        }

        public void Add<T>(string key, T obj)
        {
            _cache.Set(key, obj, _defaultCacheItemPolicy);
        }

        public void Add<T>(string key, T obj, int seconds)
        {
            _cache.Set(key, obj, DateTimeOffset.Now.AddSeconds(seconds));
        }

        public void Add<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            var cacheItemPolicy = new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration };
            _cache.Set(key, obj, cacheItemPolicy);
        }

        public bool Exists(string key)
        {
            return _cache.Get(key) != null;
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Max<T>(string key, T obj)
        {
            var cacheItemPolicy = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTime.MaxValue.AddYears(-1), Priority = CacheItemPriority.NeverRemove };
            _cache.Set(key, obj, cacheItemPolicy);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public bool Test()
        {
            const string key = "_##**Test**##_";
            const string obj = "Test";
            Add(key, obj);
            var result = Get<string>(key);
            return result == obj;
        }
    }
}