using System;
using System.Runtime.Caching;
using System.Threading;

namespace MZBlog.Core.Cache
{
    public class RuntimeCache : ICache
    {
        private readonly MemoryCache _cache;
        private readonly CacheItemPolicy _defaultCacheItemPolicy;
        public RuntimeCache()
        {
            _cache = MemoryCache.Default;
            _defaultCacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(60 * 2) };
        }

        public void Add(string key, object obj)
        {
            var cacheItem = new CacheItem(key, obj);
            _cache.Set(cacheItem, _defaultCacheItemPolicy);
        }

        public void Add(string key, object obj, int seconds)
        {
            _cache.Set(key, obj, DateTimeOffset.Now.AddSeconds(seconds));
        }

        public void Add(string key, object obj, TimeSpan slidingExpiration)
        {
            var cacheItem = new CacheItem(key, obj);
            var cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = slidingExpiration };
            _cache.Set(cacheItem, cacheItemPolicy);
        }

        public int Decrement(string key)
        {
            var cacheNumber = Get<int?>(key);
            var number = 0;
            if (cacheNumber.HasValue)
            {
                number = cacheNumber.Value;
            }
            Interlocked.Decrement(ref number);
            Max(key, number);
            return number;
        }

        public bool Exists(string key)
        {
            return _cache.Get(key) != null;
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public int Increment(string key)
        {
            var cacheNumber = Get<int?>(key);
            var number = 0;
            if (cacheNumber.HasValue)
            {
                number = cacheNumber.Value;
            }
            Interlocked.Increment(ref number);
            Max(key, number);
            return number;
        }

        public void Max(string key, object obj)
        {
            var cacheItem = new CacheItem(key, obj);
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.MaxValue.AddYears(-1), Priority = CacheItemPriority.NotRemovable };
            _cache.Set(cacheItem, cacheItemPolicy);
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