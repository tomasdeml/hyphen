using System;
using System.Collections.Generic;
using System.Text;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    internal static class RuntimeCaches
    {
        #region Fields

        private static readonly Dictionary<string, object> Caches = new Dictionary<string, object>(1);

        #endregion

        #region Getters

        public static IDictionary<TKey, TValue> GetCache<TKey, TValue>(string cacheName)
        {
            return GetCache(cacheName) as IDictionary<TKey, TValue>;   
        }

        public static object GetCache(string name)
        {
            if (Caches.ContainsKey(name))
                throw new ArgumentException("Cache not found.", "name");

            return Caches[name];
        }

        public static bool CacheRegistered(string name)
        {
            return Caches.ContainsKey(name);
        }

        #endregion

        #region Setters

        public static void RegisterCache(string name, object cache)
        {
            if (Caches.ContainsKey(name))
                throw new ArgumentException("Cache already registered.", "name");

            Caches[name] = cache;
        }

        public static bool UnregisterCache(string name)
        {
            return Caches.Remove(name);
        }

        #endregion
    }
}
