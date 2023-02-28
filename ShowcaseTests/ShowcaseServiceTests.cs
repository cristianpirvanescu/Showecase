using Microsoft.AspNetCore.Hosting;
using Moq;
using Showcase.Models;
using Showcase.Services;

namespace ShowcaseTests
{
    [TestClass]
    public class ShowcaseServiceTests
    {
        private readonly ShowcaseService _showcaseService;
        private Mock<IJsonService> _mockJsonService;
        public ShowcaseServiceTests()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("roottestfolder");
            _mockJsonService = new Mock<IJsonService>();
            _mockJsonService.Setup(m => m.ReadJsonLocally(It.IsAny<string>())).ReturnsAsync(new List<ShowcaseRoot>());
            _mockJsonService.Setup(m => m.ReadJsonFromWeb(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<ShowcaseRoot>());

            _showcaseService = new ShowcaseService(mockEnvironment.Object, _mockJsonService.Object);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task GetShowcases_ShouldReturnEmptyList_WhenJsonUrlIsNullOrEmpty(string jsonUrl)
        {
            //Act  
            var actual = await _showcaseService.GetShowcases(jsonUrl, "");

            //Assert  
            Assert.AreEqual(actual.Count(), 0);
        }

        [TestMethod]
        public async Task GetShowcases_ShouldCallReadJsonFromWeb_WhenJsonIsNotCached()
        {
            //Arrange

             //Act  
             var actual = await _showcaseService.GetShowcases("jsonfile.json", "cache");

            //Assert  
            _mockJsonService.Verify(m => m.ReadJsonFromWeb(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockJsonService.Verify(m => m.ReadJsonLocally(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetShowcases_ShouldCallReadJsonLocally_WhenJsonIsCached()
        {
            //Arrange
            Directory.CreateDirectory(Path.Combine("roottestfolder", "cache"));
            var path = Path.Combine("roottestfolder", "cache", "jsonfile.json");
            File.WriteAllText(path, "1234");

            //Act  
            var actual = await _showcaseService.GetShowcases("jsonfile.json", "cache");

            //Assert  
            _mockJsonService.Verify(m => m.ReadJsonFromWeb(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockJsonService.Verify(m => m.ReadJsonLocally(It.IsAny<string>()), Times.Once);

            //Cleanup
            File.Delete(path);
        }
    }
}