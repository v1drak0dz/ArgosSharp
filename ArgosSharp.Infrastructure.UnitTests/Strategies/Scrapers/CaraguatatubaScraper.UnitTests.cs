using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Infrastructure.Strategies.Scrapers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArgosSharp.Infrastructure.UnitTests.Strategies.Scrapers
{
    public class CaraguatatubaScraperUnitTests
    {
        private Mock<IHttpFetcher> _fetcherMock;
        private Mock<IHtmlParser> _parserMock;
        private Mock<ILogger<CaraguatatubaScraper>> _loggerMock;
        private MockRepository _mockRepository;

        private CaraguatatubaScraper _scraper;

        private const string Html = "<html>";
        private const string NewsHtml = "<news>";

        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fetcherMock = _mockRepository.Create<IHttpFetcher>();
            _parserMock = _mockRepository.Create<IHtmlParser>();
            _loggerMock = _mockRepository.Create<ILogger<CaraguatatubaScraper>>();

            _loggerMock.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(), 
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

            _scraper = new CaraguatatubaScraper(
                _fetcherMock.Object,
                _parserMock.Object,
                _loggerMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.VerifyAll();
        }

        private void SetupFetcher(params string[] urls)
        {
            _fetcherMock
                .Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(Html);
        }

        private void SetupNewsExtraction(params string[] news)
        {
            _parserMock
                .Setup(x => x.QueryTexts(Html, It.Is<string>(s => s.Contains("latestNews"))))
                .Returns(news.ToList());
        }

        private void SetupPagination(params string[] pages)
        {
            _parserMock
                .Setup(x => x.QueryTexts(Html, It.Is<string>(s => s.Contains("pagination"))))
                .Returns(pages.ToList());
        }

        private void SetupDefaultMapping(string newsItem = NewsHtml)
        {
            _parserMock
                .Setup(x => x.QueryText(newsItem, It.IsAny<string>()))
                .Returns((string _, string selector) =>
                {
                    return selector switch
                    {
                        var s when s.Contains("created-at") => "01/01/2024",
                        var s when s.Contains("a::text") => "Title test",
                        var s when s.Contains("href") => "http://link.com",
                        var s when s.Contains("news-text") => "Summary",
                        _ => null
                    };
                });
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldReturnFormattedNoticias()
        {
            // Arrange
            SetupFetcher();
            SetupPagination("1", "2", "3", "4", "5");
            SetupNewsExtraction(NewsHtml);
            SetupDefaultMapping();

            // Act
            var result = await _scraper.ProcessScraperAsync("test", 1);

            // Assert
            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Title test");
            result[0].Link.Should().Be("http://link.com");
        }

        [Test]
        public async Task ProcessScraperAsync_WhenNoPagination_ShouldReturnDefaultSinglePage()
        {
            // Arrange
            SetupFetcher();
            SetupPagination();
            SetupNewsExtraction();

            // Act
            var result = await _scraper.ProcessScraperAsync("test", 1);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldCallFetcherForPagination()
        {
            // Arrange
            SetupFetcher();
            SetupPagination("1", "2", "3", "4", "5");
            SetupNewsExtraction(NewsHtml);
            SetupDefaultMapping();

            // Act
            await _scraper.ProcessScraperAsync("test", 2);

            // Assert
            _fetcherMock.Verify(
                x => x.GetStringAsync(It.Is<string>(url => url.Contains("page"))),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldRespectDepthLimit()
        {
            // Arrange
            SetupFetcher();
            SetupPagination("1", "2", "3", "4", "5");
            SetupNewsExtraction();

            // Act
            await _scraper.ProcessScraperAsync("test", 1);

            // Assert
            _fetcherMock.Verify(
                x => x.GetStringAsync(It.Is<string>(url => url.Contains("page"))),
                Times.AtMost(1));
        }

        [Test]
        public async Task ProcessScraperAsync_WhenNoNews_ShouldReturnEmptyList()
        {
            // Arrange
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNewsExtraction();

            // Act
            var result = await _scraper.ProcessScraperAsync("test", 1);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldUseFallbackValues_WhenMissingFields()
        {
            // Arrange
            SetupFetcher();
            SetupPagination("1", "2", "3");

            SetupNewsExtraction(NewsHtml);

            _parserMock
                .Setup(x => x.QueryText(NewsHtml, It.IsAny<string>()))
                .Returns((string _, string selector) =>
                {
                    return selector.Contains("created-at") ? "01/01/2024" : null;
                });

            // Act
            var result = await _scraper.ProcessScraperAsync("test", 1);

            // Assert
            result[0].Title.Should().Be("No title");
            result[0].Link.Should().Be("No link");
        }

        [Test]
        public async Task ProcessScraperAsync_WhenInvalidDate_ShouldNotThrow()
        {
            // Arrange
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNewsExtraction(NewsHtml);

            _parserMock
                .Setup(x => x.QueryText(NewsHtml, It.IsAny<string>()))
                .Returns("invalid-date");

            // Act
            Func<Task> act = () => _scraper.ProcessScraperAsync("test", 1);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task ProcessScraperAsync_WhenFetcherFails_ShouldThrowException()
        {
            // Arrange
            _fetcherMock
                .Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            Func<Task> act = () => _scraper.ProcessScraperAsync("test", 1);

            // Assert
            await act
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Error");
        }
    }
}