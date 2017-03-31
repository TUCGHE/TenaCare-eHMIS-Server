using System;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using System.Data;

namespace eHMISWebApi.Caching
{
    public class MemcachedClientWrapper : ICacheClient
    {
        private IMemcachedClient _client;

        public MemcachedClientWrapper()
        {
            var config = new MemcachedClientConfiguration();
            config.AddServer("127.0.0.1:11211");
            // Need to include the configuration in web.config
            _client = new MemcachedClient(config);
        }
        public void AddToCache(string key, object data)
        {
            //_client.Cas(Enyim.Caching.Memcached.StoreMode.Set, key, data);
            _client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, data, DateTime.Now.AddMinutes(30));
        }

        public void AddToCacheCompressed(string key, object data)
        {
            var compressedData = SerializerHelper.Serialize(data);
            AddToCache(key, compressedData);
        }

        public T GetFromCache<T>(string key, Func<T> getData = null)
        {
            object data;
            if (_client.TryGet(key, out data))
                return (T)data;

            if (getData != null)
            {
                var newData = getData();
                AddToCache(key, data);

                return newData;
            }

            return default(T);
        }

        public T GetFromCacheCompressed<T>(string key)
        {
            var compressedData = GetFromCache<byte[]>(key);
            var uncompressedData = SerializerHelper.Deserialize<SerializableObject>(compressedData);

            var actualData = (T)uncompressedData.Data;

            return actualData;
        }

        public void RemoveFromCache(string key)
        {
            _client.Remove(key);
        }

        public void ReplaceValueInCache(string key, object data)
        {
            _client.Store(Enyim.Caching.Memcached.StoreMode.Replace, key, data);
        }

        public void ReplaceValueInCacheCompressed(string key, object data)
        {
            throw new NotImplementedException();
        }
    }
}
