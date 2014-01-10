using System;

namespace MZBlog.Core.Cache
{
    public interface ICache
    {
        /// <summary>
        /// 向Cache中添加缓存对象
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">要缓存的对象</param>
        void Add(string key, object obj);

        /// <summary>
        /// 向Cache中添加缓存对象，如果时间达到指定的秒数即被移除
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">要缓存的对象</param>
        /// <param name="seconds">固定的超时秒数</param>
        void Add(string key, object obj, int seconds);

        /// <summary>
        /// 向Cache中添加缓存对象
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">要缓存的对象</param>
        /// <param name="slidingExpiration">指定时间段</param>
        void Add(string key, object obj, TimeSpan slidingExpiration);

        /// <summary>
        /// 对指定缓存Key的整数进行原子递减操作
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>递减增后的整数</returns>
        int Decrement(string key);

        /// <summary>
        /// 从Cache中判断指定的Key是否已经有缓存数据
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>true/false</returns>
        bool Exists(string key);

        /// <summary>
        /// 从Cache中获取缓存的对象
        /// </summary>
        /// <typeparam name="T">缓存对象的类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存的对象</returns>
        T Get<T>(string key);

        /// <summary>
        /// 对指定缓存Key的整数进行原子递增操作
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>递增后的整数</returns>
        int Increment(string key);

        /// <summary>
        /// 向Cache中永久缓存对象
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">要缓存的对象</param>
        void Max(string key, object obj);

        /// <summary>
        /// 从Cache中移除缓存项
        /// </summary>
        /// <param name="key">缓存Key</param>
        void Remove(string key);

        /// <summary>
        /// 测试Cache是否可用
        /// </summary>
        /// <returns></returns>
        bool Test();
    }
}