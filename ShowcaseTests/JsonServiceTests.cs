using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Showcase.Models;
using Showcase.Services;

namespace ShowcaseTests
{
    [TestClass]
    public class JsonServiceTests
    {
        private readonly JsonService _jsonService;
        private Mock<IDataHandler> _mockDataHandler;
        public JsonServiceTests()
        {
            _mockDataHandler = new Mock<IDataHandler>();
            _mockDataHandler.Setup(m => m.DownloadJson(It.IsAny<string>())).ReturnsAsync(new List<ShowcaseRoot> {new ShowcaseRoot() });
            _mockDataHandler.Setup(m => m.CacheItems(It.IsAny<List<ShowcaseRoot>>(), It.IsAny<string>()));

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            _jsonService = new JsonService(_mockDataHandler.Object, memoryCache);
        }

        [TestMethod]
        public async Task ReadJsonFromWeb_ShouldCallProperDataHandlerMethods()
        {
            //Arrange

             //Act  
             var actual = await _jsonService.ReadJsonFromWeb("jsonfile.json", "cache", "jsonlocalfile.json");

            //Assert  
            _mockDataHandler.Verify(m => m.DownloadJson(It.IsAny<string>()), Times.Once);
            _mockDataHandler.Verify(m => m.CacheItems(It.IsAny<List<ShowcaseRoot>>(), It.IsAny<string>()), Times.Once);
        }
    }
}