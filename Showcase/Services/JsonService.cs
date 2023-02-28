using System.Net.Cache;
using System.Net;
using System.Text.Json;
using Showcase.Models;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.IO;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace Showcase.Services
{
    public class JsonService : IJsonService
    {
        private readonly IDataHandler _dataHandler;
        private readonly IMemoryCache _memoryCache;


        public JsonService(IDataHandler dataHandler, IMemoryCache memoryCache)
        {
            _dataHandler = dataHandler ?? throw new ArgumentNullException(nameof(dataHandler));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache)); 
        }
        
        public async Task<List<ShowcaseRoot>> ReadJsonLocally(string path)
        {   
            if (!_memoryCache.TryGetValue(path, out List<ShowcaseRoot> cacheValue))
            {
                List<ShowcaseRoot> showcases;
                using (StreamReader r = new StreamReader(path))
                {
                    string json = await r.ReadToEndAsync();
                    showcases = JsonSerializer.Deserialize<List<ShowcaseRoot>>(json);
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3600));

                _memoryCache.Set(path, showcases, cacheEntryOptions);
                return showcases;
            }

           return cacheValue;            
        }

        public async Task<List<ShowcaseRoot>> ReadJsonFromWeb(string jsonUrl, string cacheFolder, string jsonLocalPath)
        {
            var showcases = await _dataHandler.DownloadJson(jsonUrl);
            await _dataHandler.CacheItems(showcases, cacheFolder);
            var json = JsonSerializer.Serialize(showcases);
            await File.WriteAllTextAsync(jsonLocalPath, json);
            return showcases;
        }

    }
}

