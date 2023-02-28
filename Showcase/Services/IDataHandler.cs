using System.Net.Cache;
using System.Net;
using System.Text.Json;
using Showcase.Models;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.IO;
using Microsoft.Extensions.Options;

namespace Showcase.Services
{
    public interface IDataHandler
    {        
        public Task<List<ShowcaseRoot>> DownloadJson(string jsonUrl);
        public Task CacheItems(List<ShowcaseRoot> showcases, string cacheFolder);
        public Task CacheItem(ShowcaseRoot showcase, string cacheFolder);
        public Task DownloadAndCacheMedia<T>(T mediaItem, string cacheFolder) where T : EntityWithUrl;
    }
}

