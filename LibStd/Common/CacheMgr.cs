using Microsoft.Extensions.Caching.Memory;
using System;

namespace Tracker.Web
{
    /// <summary>
    /// class for application-wide caching in HttpContext(for web) or Memory cache (for client)
    /// </summary>
    public class CacheMgr
    {

      
        private static IMemoryCache _memCache; //needs to be assigned at the HomeController constructor, where it should be DI-d
       

        
       
        public CacheMgr(IMemoryCache memCache)
        {
            if (_memCache==null)
            {
                _memCache = memCache;
            }
        }
        /// <summary>
        /// get from cache and set to permanent cache with key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }
        /// <summary>
        /// get object from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            object value;
            if (_memCache.TryGetValue(key,out value))
            {
                return value;
            }
            return null;

        }
        /// <summary>
        /// get typed object from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            T value;
            if (_memCache.TryGetValue<T>(key, out value))
            {
                return value;
            }
            
            return default(T);

        }
        /// <summary>
        /// set object into cache permanently
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Set(string key, object obj)
        {
            
                _memCache.Set(key, obj);
            
        }
        /// <summary>
        /// put object into cache with expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="isSlidingExpiration">is expiration sliding?</param>
        /// <param name="expirationMinutes">if 0 or less- set into permanent cache</param>
        public static void Set(string key, object obj, bool isSlidingExpiration, int expirationMinutes)
        {
            DateTimeOffset absoluteExpiration;
            TimeSpan slidingExpiration;
            if (expirationMinutes <= 0)
            {
                Set(key, obj);
                return;
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            if (isSlidingExpiration)
            {
                slidingExpiration = TimeSpan.FromMinutes(expirationMinutes);
               cacheEntryOptions.SetSlidingExpiration(slidingExpiration); // Keep in cache for this time, reset time if accessed.
                
                
              
               
            }
            else
            {
                absoluteExpiration = DateTime.Now.AddMinutes(expirationMinutes);

                // Keep in cache for this time
                cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration);
            }
            _memCache.Set(key, obj, cacheEntryOptions);

        }

        /// <summary>
        /// remove object from cache
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
           
                _memCache.Remove(key);
            
        }

        /// <summary>
        /// Remove all keys from cache starting with thye startKey
        /// </summary>
        /// <param name="keyStart"></param>
        public static void InvalidateCache(string keyStart)
        {
            //TODO
            //That doesn't work. Need to set items as depenant of other item and then remove this top item

        }
        
    }
}
