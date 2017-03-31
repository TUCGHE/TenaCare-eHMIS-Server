using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eHMISWebApi.Caching
{
    public interface ICacheClient
    {
        void AddToCache(string key, object data);
        void RemoveFromCache(string key);
        void ReplaceValueInCache(string key, object data);
        T GetFromCache<T>(string key, Func<T> getData);

        void AddToCacheCompressed(string key, object data);
        void ReplaceValueInCacheCompressed(string key, object data);
        T GetFromCacheCompressed<T>(string key);
    }
}
