using Microsoft.Extensions.Caching.Memory;

namespace Mio.Translation
{
    internal class Cache<T>
    {
        readonly MemoryCache memoryCache;
        readonly MemoryCacheEntryOptions cacheEntryOptions;

        public Cache (int sizeLimit)
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = sizeLimit
            });

            cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
        }

        public async Task<(bool,T?)> TryGetIndex(int index)
        {
            T? value;
            bool wasThere = memoryCache.TryGetValue(index, out value);
            return (wasThere,value);
        }

        public async Task Insert(int key, T value)
        {
            memoryCache.Set(key, value,cacheEntryOptions);
        }
    }
}
