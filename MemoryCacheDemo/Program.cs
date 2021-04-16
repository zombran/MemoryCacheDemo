using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;

namespace MemoryCacheDemo
{
    class Program
    {
        private static IMemoryCache cache;
        private static byte[] cacheEntry;

        static void Main(string[] args)
        {  
            cache = BuildDefaultCache();
            //cacheEntry = InitializeCacheEntry(536870912); // .5GB
            cacheEntry = InitializeCacheEntry(1073741824); // 1GB
            FloodCache();
            Console.ReadLine();
        }

        private static byte[] InitializeCacheEntry(int size)
        {
            Console.WriteLine("Building a demo cache entry.");
            byte[] entry = new byte[size];
            for(int i = 0; i < size; i++)
            {
                entry[i] = byte.MaxValue;
            }
            Console.WriteLine($"Demo cache entry built with {size} bytes.");
            return entry;
        }

        private static IMemoryCache BuildDefaultCache()
        {
            Console.WriteLine("Initializing default cache.");
            return new MemoryCache(new MemoryCacheOptions());
        }

        private static void FloodCache()
        {
            int sleepMilliseconds = 500;
            Console.WriteLine($"Flooding the cache with a new item every {sleepMilliseconds}ms....");
            int count = 1;

            while (true)
            {
                MemoryCacheEntryOptions entryOptions = new MemoryCacheEntryOptions()
                {                
                    Priority = CacheItemPriority.Normal,
                    Size = cacheEntry.Length,
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                }
                .RegisterPostEvictionCallback((key, value, reason, state) => {
                    Console.WriteLine($"Eviction fired for '{key}' and reason '{reason}.");
                });

                Guid guid = Guid.NewGuid();
                Console.WriteLine($"Adding entry number {count} with key '{guid.ToString()}.");
                cache.Set(guid, cacheEntry.Clone(), entryOptions);
                
                count++;
                Thread.Sleep(sleepMilliseconds);
            }
        }
    }
}
