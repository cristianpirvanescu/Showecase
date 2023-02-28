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
    public interface IJsonService
    {
        Task<List<ShowcaseRoot>> ReadJsonLocally(string path);
        public Task<List<ShowcaseRoot>> ReadJsonFromWeb(string jsonUrl, string cacheFolder, string jsonLocalPath);
    }
}

