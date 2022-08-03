using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace Bakery.Utils
{
    public static class MemoryCacher
    {
        public enum CacheItemPriority
        {
            Low = 0,
            Normal = 1,
            High = 2,
            NeverRemove = 3
        }

        public class KeyChangeToken : IChangeToken
        {
            internal static readonly ConcurrentDictionary<object, List<object>> _DependencyOnKeys =
                new ConcurrentDictionary<object, List<object>>();

            internal readonly object _MKey;

            public KeyChangeToken(object key)
            {
                _MKey = key;
            }

            public object Key => _MKey;

            public bool HasChanged => false;

            public bool ActiveChangeCallbacks => false;

            public IDisposable RegisterChangeCallback(Action<object> callback, object state)
            {
                return default(IDisposable);
            }
        }


        private static readonly MemoryCache _MemoryCache;

        private static readonly Func<MemoryCache, IDictionary> _EntriesFunc;

        static MemoryCacher()
        {
            //超过某个时间段扫描所有缓存移除过期缓存,但是不会有定时器自动执行,只有访问了缓存类且上次扫描时间超过时间段才会执行,相对鸡肋.
            var mcOptions = new MemoryCacheOptions {ExpirationScanFrequency = TimeSpan.FromHours(1)};
            _MemoryCache = new MemoryCache(mcOptions);

            
            //Lambda缓存字段读取方法，比字段反射取值效率高一倍以上
            var entriesField = typeof(MemoryCache).GetField("_entries",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var DeclaringExp = Expression.Parameter(typeof(MemoryCache), "obj");
            var PropertyExp = Expression.Field(DeclaringExp, entriesField);
            var PropertyConvertExp = Expression.Convert(PropertyExp, typeof(IDictionary));
            _EntriesFunc = Expression.Lambda<Func<MemoryCache, IDictionary>>(PropertyConvertExp, DeclaringExp).Compile();
        }


        /// <summary>
        /// 缓存键列表
        /// </summary>
        public static List<object> Keys => Entries.Keys.ToList();

        /// <summary>
        /// 获取所有缓存
        /// </summary>
        /// <returns></returns>
        public static IDictionary<object, object> Entries
        {
            get
            {
                var res = new Dictionary<object, object>();

                var cacheItems = _EntriesFunc(_MemoryCache);
                if (cacheItems == null) return res;
                foreach (DictionaryEntry cacheItem in cacheItems)
                {
                    res.Add(cacheItem.Key, (cacheItem.Value as ICacheEntry)?.Value);
                }

                return res;
            }
        }

        /// <summary>
        /// 缓存总项数
        /// </summary>
        public static long Count => _MemoryCache.Count;

        // /// <summary>
        // /// 获取计算机上缓存可使用的内存量（以字节为单位）。
        // /// </summary>
        // public static long CacheMemoryLimit
        // {
        //     get { return _MemoryCache.CacheMemoryLimit; }
        // }

        ///// <summary>
        ///// 获取缓存可使用的物理内存的百分比。
        ///// </summary>
        //public static long PhysicalMemoryLimit
        //{
        //    get { return _MemoryCache.PhysicalMemoryLimit; }
        //}

        ///// <summary>
        ///// 获取在缓存更新其内存统计信息之前需等待的最大时间量。
        ///// </summary>
        //public static TimeSpan PollingInterval
        //{
        //    get { return _MemoryCache.PollingInterval; }
        //}

        /// <summary>
        /// 从缓存中移除指定百分比的缓存项
        /// </summary>
        /// <param name="p_percent">百分比 不得大于100</param>
        /// <returns>返回被移除的项数</returns>
        public static void Trim(double p_percent)
        {
            _MemoryCache.Compact(p_percent);
        }

        //public static ICacheEntry CreateEntry(object p_Key)
        //{
        //    if (p_Key != null)
        //    {
        //        object val;
        //        if (m_memoryCache.TryGetValue(p_Key, out val))
        //        {
        //            KeysCheck2Add(p_Key);
        //            return m_memoryCache.CreateEntry(p_Key);
        //        }
        //        else
        //        {
        //            KeysCheck2Del(p_Key);
        //        }
        //    }
        //    return default(ICacheEntry);

        //}

        /// <summary>
        /// 获取对应缓存ID的缓存内容
        /// </summary>
        /// <typeparam name="T">缓存内容的类型</typeparam>
        /// <param name="p_key">缓存ID</param>
        /// <returns>缓存内容</returns>
        public static T Get<T>(object p_key)
        {
            if (p_key != null)
            {
                if (_MemoryCache.TryGetValue<T>(p_key, out var val))
                {
                    return val;
                }
            }

            return default(T);
        }

        /// <summary>
        /// 获取对应缓存ID的缓存内容
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        /// <returns>缓存内容</returns>
        public static object Get(object p_key)
        {
            if (p_key != null)
            {
                if (_MemoryCache.TryGetValue(p_key, out var val))
                {
                    return val;
                }
            }

            return default(object);
        }

        /// <summary>
        /// 获取对应缓存ID的缓存内容
        /// </summary>
        /// <typeparam name="T">缓存内容的类型</typeparam>
        /// <param name="p_key">缓存ID</param>
        /// <param name="p_value">缓存内容</param>
        /// <returns>是否存在</returns>
        public static bool TryGet<T>(object p_key, out T p_value)
        {
            return _MemoryCache.TryGetValue<T>(p_key, out p_value);
        }

        /// <summary>
        /// 获取对应缓存ID的缓存内容
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        /// <param name="p_value">缓存内容</param>
        /// <returns>是否存在</returns>
        public static bool TryGet(object p_key, out object p_value)
        {
            return _MemoryCache.TryGetValue(p_key, out p_value);
        }

        /// <summary>
        /// 对应缓存ID的缓存是否存在
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public static bool Contains(object p_key)
        {
            return _MemoryCache.TryGetValue(p_key, out object value);
        }

        /// <summary>
        /// 获取某个标识开头的缓存
        /// </summary>
        /// <param name="p_keyStartsWith"></param>
        /// <returns></returns>
        public static IDictionary<object, object> GetByKeyStartWith(string p_keyStartsWith)
        {
            return Entries.Where(p => p.Key.ToString().StartsWith(p_keyStartsWith, StringComparison.Ordinal))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private static List<object> GetKeysByKeyStartWith(string p_keyStartsWith)
        {
            return Keys.Where(p => p.ToString().StartsWith(p_keyStartsWith, StringComparison.Ordinal)).ToList();
        }

        /// <summary>
        /// 获取包含某个标识的缓存
        /// </summary>
        /// <param name="p_keyContains"></param>
        /// <returns></returns>
        public static IDictionary<object, object> GetByKeyContains(string p_keyContains)
        {
            return Entries.Where(p => p.Key.ToString().IndexOf(p_keyContains, StringComparison.Ordinal) > -1)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private static List<object> GetKeysByKeyContains(string p_keyContains)
        {
            return Keys.Where(p => p.ToString().IndexOf(p_keyContains, StringComparison.Ordinal) > -1).ToList();
        }


        /// <summary>
        /// 删除key缓存
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        public static void Remove(object p_key)
        {
            if (p_key != null)
            {
                if (_MemoryCache.TryGetValue(p_key, out _))
                {
                    _MemoryCache.Remove(p_key);
                }
            }
        }

        /// <summary>
        /// 清除某个标识开头的缓存
        /// </summary>
        /// <param name="p_keyStartsWith">缓存ID开头标识</param>
        public static void RemoveByKeyStartWith(string p_keyStartsWith)
        {
            foreach (var cachekey in GetKeysByKeyContains(p_keyStartsWith))
            {
                _MemoryCache.Remove(cachekey);
            }
        }

        /// <summary>
        /// 清除包含某个标识的缓存
        /// </summary>
        /// <param name="p_keyContains">缓存ID标识</param>
        public static void RemoveByKeyContains(string p_keyContains)
        {
            foreach (var cachekey in GetKeysByKeyContains(p_keyContains))
            {
                _MemoryCache.Remove(cachekey);
            }
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public static void Clear()
        {
            foreach (var key in Keys)
            {
                _MemoryCache.Remove(key);
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        /// <param name="p_value">缓存内容</param>
        /// <param name="p_cacheItemPriority">缓存的相对优先级</param>
        /// <param name="p_absoluteExpiration">绝对到期时间(不可与相对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_slidingExpiration">相对到期时间(不可与绝对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_onRemovedCallback">缓存移除事件</param>
        public static void Set(object p_key, object p_value,
            CacheItemPriority p_cacheItemPriority = CacheItemPriority.Low, DateTimeOffset? p_absoluteExpiration = null,
            TimeSpan? p_slidingExpiration = null,
            Action<object, object, EvictionReason, object> p_onRemovedCallback = null)
        {
            Set(p_key, p_value, p_cacheItemPriority, p_absoluteExpiration, p_slidingExpiration, null,
                p_onRemovedCallback);
        }

        /// <summary>
        /// 设置缓存依赖现有目录及文件后缀名
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        /// <param name="p_value">缓存内容</param>
        /// <param name="p_dependenciesDirectorys">缓存依赖目录及文件后缀名</param>
        /// <param name="p_cacheItemPriority">缓存的相对优先级</param>
        /// <param name="p_absoluteExpiration">绝对到期时间(不可与相对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_slidingExpiration">相对到期时间(不可与绝对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_onRemovedCallback">缓存移除事件</param>
        public static void SetByDependenciesDirectorys(string p_key, dynamic p_value,
            IEnumerable<KeyValuePair<string, string>> p_dependenciesDirectorys = null,
            CacheItemPriority p_cacheItemPriority = CacheItemPriority.Low, DateTimeOffset? p_absoluteExpiration = null,
            TimeSpan? p_slidingExpiration = null,
            Action<object, object, EvictionReason, object> p_onRemovedCallback = null)
        {
            var ctlist = new List<IChangeToken>();
            var ct = DependencyOnDirectorys(p_dependenciesDirectorys.ToArray());
            if (ct != null)
            {
                ctlist.Add(ct);
            }

            Set(p_key, p_value, p_cacheItemPriority, p_absoluteExpiration, p_slidingExpiration,
                ctlist,
                p_onRemovedCallback);
        }

        /// <summary>
        /// 设置缓存依赖现有文件
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        /// <param name="p_value">缓存内容</param>
        /// <param name="p_dependenciesFiles">缓存依赖文件列表</param>
        /// <param name="p_cacheItemPriority">缓存的相对优先级</param>
        /// <param name="p_absoluteExpiration">绝对到期时间(不可与相对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_slidingExpiration">相对到期时间(不可与绝对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_onRemovedCallback">缓存移除事件</param>
        public static void SetByDependenciesFiles(string p_key, dynamic p_value,
            IEnumerable<string> p_dependenciesFiles, CacheItemPriority p_cacheItemPriority = CacheItemPriority.Low,
            DateTimeOffset? p_absoluteExpiration = null, TimeSpan? p_slidingExpiration = null,
            Action<object, object, EvictionReason, object> p_onRemovedCallback = null)
        {
            var ctlist = new List<IChangeToken>();
            var ct = DependencyOnFiles(p_dependenciesFiles.ToArray());
            if (ct != null)
            {
                ctlist.Add(ct);
            }

            Set(p_key, p_value, p_cacheItemPriority, p_absoluteExpiration, p_slidingExpiration,
                ctlist,
                p_onRemovedCallback);
        }

        /// <summary>
        /// 设置缓存依赖现有缓存键
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        /// <param name="p_value">缓存内容</param>
        /// <param name="p_dependenciesKeys">缓存依赖键列表</param>
        /// <param name="p_cacheItemPriority">缓存的相对优先级</param>
        /// <param name="p_absoluteExpiration">绝对到期时间(不可与相对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_slidingExpiration">相对到期时间(不可与绝对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_onRemovedCallback">缓存移除事件</param>
        public static void SetByDependenciesKeys(string p_key, dynamic p_value, IEnumerable<string> p_dependenciesKeys,
            CacheItemPriority p_cacheItemPriority = CacheItemPriority.Low, DateTimeOffset? p_absoluteExpiration = null,
            TimeSpan? p_slidingExpiration = null,
            Action<object, object, EvictionReason, object> p_onRemovedCallback = null)
        {
            var ctlist = new List<IChangeToken>();
            var ct = DependencyOnKeys(p_dependenciesKeys.ToArray());
            if (ct != null)
            {
                ctlist.Add(ct);
            }

            Set(p_key, p_value, p_cacheItemPriority, p_absoluteExpiration, p_slidingExpiration,
                ctlist,
                p_onRemovedCallback);
        }

        public static IChangeToken DependencyOnDirectorys(params KeyValuePair<string, string>[] p_dirPatterns)
        {
            if (null != p_dirPatterns && p_dirPatterns.Any())
            {
                var ctlist = new List<IChangeToken>();
                foreach (var item in p_dirPatterns)
                {
                    if (Directory.Exists(item.Key))
                    {
                        ctlist.Add(new PollingWildCardChangeToken(item.Key, item.Value));
                    }
                }

                if (ctlist.Any())
                    return new CompositeChangeToken(ctlist);
            }

            return null;
        }

        public static IChangeToken DependencyOnFiles(params string[] files)
        {
            if (null != files && files.Any())
            {
                var ctlist = new List<IChangeToken>();
                foreach (var item in files)
                {
                    if (File.Exists(item))
                    {
                        ctlist.Add(new PollingFileChangeToken(new FileInfo(item)));
                    }
                }

                if (ctlist.Any())
                    return new CompositeChangeToken(ctlist);
            }

            return null;
        }

        public static IChangeToken DependencyOnKeys(params object[] keys)
        {
            if (null != keys && keys.Any())
            {
                var ctlist = new List<IChangeToken>();
                foreach (var item in keys)
                {
                    if (Keys.Contains(item))
                    {
                        ctlist.Add(new KeyChangeToken(item));
                    }
                }

                if (ctlist.Any())
                    return new CompositeChangeToken(ctlist);
            }

            return null;
        }


        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="p_key">缓存ID</param>
        /// <param name="p_value">缓存内容</param>
        /// <param name="p_cacheItemPriority">缓存的相对优先级</param>
        /// <param name="p_absoluteExpiration">绝对到期时间(不可与相对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_slidingExpiration">相对到期时间(不可与绝对到期时间同时设置,不设置的话设置为null)</param>
        /// <param name="p_changeTokens">缓存依赖项</param>
        /// <param name="p_onRemovedCallback">缓存移除事件</param>
        public static void Set(object p_key, object p_value, CacheItemPriority p_cacheItemPriority,
            DateTimeOffset? p_absoluteExpiration, TimeSpan? p_slidingExpiration,
            IEnumerable<IChangeToken> p_changeTokens,
            Action<object, object, EvictionReason, object> p_onRemovedCallback = null)
        {
            if (null != p_key)
            {
                if (null != p_value)
                {
                    var mceo = new MemoryCacheEntryOptions();
                    mceo.Priority = ((Microsoft.Extensions.Caching.Memory.CacheItemPriority) (int) p_cacheItemPriority);

                    if (p_absoluteExpiration != null && p_slidingExpiration == null)
                    {
                        mceo.AbsoluteExpiration = (DateTimeOffset) p_absoluteExpiration;
                    }
                    else if (p_slidingExpiration != null && p_absoluteExpiration == null)
                    {
                        mceo.SlidingExpiration = (TimeSpan) p_slidingExpiration;
                    }
                    else if (p_slidingExpiration == null && p_absoluteExpiration == null)
                    {
                        mceo.SlidingExpiration = TimeSpan.FromMinutes(30);
                    }
                    else if (p_slidingExpiration != null && p_absoluteExpiration != null)
                    {
                        mceo.AbsoluteExpiration = (DateTimeOffset) p_absoluteExpiration;
                        mceo.SlidingExpiration = (TimeSpan) p_slidingExpiration;
                    }

                    if (p_onRemovedCallback != null)
                        mceo.RegisterPostEvictionCallback(new PostEvictionDelegate(p_onRemovedCallback));


                    if (p_changeTokens != null && p_changeTokens.Any())
                    {
                        var keys = new List<object>();
                        foreach (var item in p_changeTokens)
                        {
                            if (item is CompositeChangeToken &&
                                ((CompositeChangeToken) item).ChangeTokens.All(p => p is KeyChangeToken))
                            {
                                foreach (var token in ((CompositeChangeToken) item).ChangeTokens.Where(p =>
                                    p is KeyChangeToken))
                                {
                                    keys.Add(((KeyChangeToken) token).Key);
                                }
                            }
                            else if (item is KeyChangeToken)
                            {
                                keys.Add(((KeyChangeToken) item).Key);
                            }

                            mceo.AddExpirationToken(item);
                        }

                        if (keys.Count > 0)
                        {
                            foreach (var fatherkey in keys)
                            {
                                if (KeyChangeToken._DependencyOnKeys.ContainsKey(fatherkey))
                                {
                                    KeyChangeToken._DependencyOnKeys[fatherkey].Add(p_key);
                                }
                                else
                                {
                                    KeyChangeToken._DependencyOnKeys[fatherkey] = new List<object> {p_key};
                                }
                            }
                        }
                    }

                    mceo.RegisterPostEvictionCallback((echoKey, value, reason, state) =>
                    {
                        if (KeyChangeToken._DependencyOnKeys.ContainsKey(echoKey))
                        {
                            foreach (var item in KeyChangeToken._DependencyOnKeys[echoKey])
                            {
                                _MemoryCache.Remove(item);
                            }

                            List<object> val;
                            KeyChangeToken._DependencyOnKeys.TryRemove(echoKey, out val);
                        }
                    });

                    _MemoryCache.Set(p_key, p_value, mceo);
                }
            }
            else
            {
                throw new ArgumentNullException("p_key");
            }
        }
    }
}