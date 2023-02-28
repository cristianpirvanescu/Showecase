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
using System;

namespace Showcase.Services
{
    public class DataHandler : IDataHandler
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpClientFactory _httpClientFactory;

        public DataHandler(IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<List<ShowcaseRoot>> DownloadJson(string jsonUrl)
        {
            string jsonString;

            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(jsonUrl))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader reader = new StreamReader(streamToReadFrom))
                    {
                        jsonString = await reader.ReadToEndAsync();
                    }
                }
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            List<ShowcaseRoot> showcases = JsonSerializer.Deserialize<List<ShowcaseRoot>>(jsonString, options);
            return showcases;
        }


        public async Task CacheItems(List<ShowcaseRoot> showcases, string cacheFolder)
        {
            Directory.CreateDirectory(cacheFolder);
            List<Task> tasks = new List<Task>();

            foreach (var showcase in showcases)
            {
                tasks.Add(CacheItem(showcase, cacheFolder));
            }
            await Task.WhenAll(tasks);
        }

        public async Task CacheItem(ShowcaseRoot showcase, string cacheFolder)
        {
            foreach (var anImg in showcase.Galleries ?? new List<Gallery>())
            {
                await DownloadAndCacheMedia(anImg, cacheFolder);
            }

            foreach (var anImg in showcase.CardImages ?? new List<CardImage>())
            {
                await DownloadAndCacheMedia(anImg, cacheFolder);
            }

            foreach (var anImg in showcase.KeyArtImages ?? new List<KeyArtImage>())
            {
                await DownloadAndCacheMedia(anImg, cacheFolder);
            }

            foreach (var aVideo in showcase.Videos ?? new List<Video>())
            {
                await DownloadAndCacheMedia(aVideo, cacheFolder);
                foreach (var aVideoAlt in aVideo.Alternatives ?? new List<Alternative>())
                {
                    await DownloadAndCacheMedia(aVideoAlt, cacheFolder);

                }
            }
        }

        public async Task DownloadAndCacheMedia<T>(T mediaItem, string cacheFolder) where T : EntityWithUrl
        {
            if (!string.IsNullOrWhiteSpace(mediaItem?.Url))
            {
                var imagePath = Path.Combine(cacheFolder, mediaItem.Url.Split('/').Last());
                if (!File.Exists(imagePath))
                {
                    using (var client = _httpClientFactory.CreateClient())
                    {
                        try
                        {
                            var imageBytes = await client.GetByteArrayAsync(mediaItem.Url);
                            await File.WriteAllBytesAsync(imagePath, imageBytes);
                            mediaItem.Url = imagePath.Replace(_webHostEnvironment.WebRootPath, "");
                        }
                        catch
                        {
                            mediaItem.Url = "";
                        }
                    }
                }
                else
                {
                    mediaItem.Url = imagePath.Replace(_webHostEnvironment.WebRootPath, "");
                }
            }

            if (typeof(T).BaseType.Name == nameof(EntityWithUrlAndThumbnail))
            {
                if (!string.IsNullOrWhiteSpace((mediaItem as EntityWithUrlAndThumbnail)?.ThumbnailUrl))
                {
                    var imagePath = Path.Combine(cacheFolder, (mediaItem as EntityWithUrlAndThumbnail).ThumbnailUrl.Split('/').Last());
                    if (!File.Exists(imagePath))
                    {
                        using (var client = _httpClientFactory.CreateClient())
                        {
                            try
                            {
                                var imageBytes = await client.GetByteArrayAsync((mediaItem as EntityWithUrlAndThumbnail).ThumbnailUrl);
                                await File.WriteAllBytesAsync(imagePath, imageBytes);
                                (mediaItem as EntityWithUrlAndThumbnail).ThumbnailUrl = imagePath.Replace(_webHostEnvironment.WebRootPath, "");
                            }
                            catch
                            {
                                (mediaItem as EntityWithUrlAndThumbnail).ThumbnailUrl = "";
                            }
                        }
                    }
                    else
                    {
                        (mediaItem as EntityWithUrlAndThumbnail).ThumbnailUrl = imagePath.Replace(_webHostEnvironment.WebRootPath, "");
                    }
                }
            }
        }
    }
}

