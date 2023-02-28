using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Showcase.Models;
using Showcase.Services;
using System.Diagnostics;
using System.Net.Mime;

namespace Showcase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IShowcaseService _jsonParser;
        private readonly IConfiguration _configuration;
        private readonly string _jsonUrl;
        private readonly string _cacheFolder;
        public HomeController(ILogger<HomeController> logger, IShowcaseService jsonParser, IConfiguration configuration)
        {
            _logger = logger;
            _jsonParser = jsonParser;
            _configuration = configuration;
            _jsonUrl = _configuration.GetValue<string>("JsonUrl");
            _cacheFolder = _configuration.GetValue<string>("CacheFolder");
        }

        public async Task<IActionResult> Index()
        {
            
            var showcases = await _jsonParser.GetShowcases(_jsonUrl, _cacheFolder);
            ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString();
            return View(showcases);
        }

        public async Task<IActionResult> Details(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Invalid movie id";
                return RedirectToAction("Index");
            }
            var showcase = (await _jsonParser.GetShowcases(_jsonUrl, _cacheFolder)).Where(q=>q.Id == id).FirstOrDefault();
            if(showcase is null)
            {
                TempData["ErrorMessage"] = "Invalid movie id";
                return RedirectToAction("Index");
            }
            return View(showcase);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}