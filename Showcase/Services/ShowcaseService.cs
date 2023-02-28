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
    public class ShowcaseService : IShowcaseService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IJsonService _jsonService;
        
        public ShowcaseService(IWebHostEnvironment webHostEnvironment, IJsonService jsonService)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService));
        }
        public async Task<List<ShowcaseRoot>> GetShowcases(string jsonUrl, string cacheFolder)
        {
            List<ShowcaseRoot> showcases = new List<ShowcaseRoot>();
            if (!string.IsNullOrWhiteSpace(jsonUrl))
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, cacheFolder, jsonUrl.Split('/').Last());
                if (!File.Exists(path))
                {
                    showcases = await _jsonService.ReadJsonFromWeb(jsonUrl, Path.Combine(_webHostEnvironment.WebRootPath, cacheFolder), path);
                }
                else
                {
                    showcases = await _jsonService.ReadJsonLocally(path);
                }
            }
            return showcases;
        }       
    }
}

