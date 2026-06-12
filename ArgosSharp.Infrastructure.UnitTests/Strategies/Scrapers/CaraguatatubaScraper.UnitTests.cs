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

        private CaraguatatubaScraper _scraper;

        private const string Html = "<html>";
        private const string NewsHtml = "<news>";

        [SetUp]
        public void Setup()
        {
            _fetcherMock = new Mock<IHttpFetcher>();
            _parserMock = new Mock<IHtmlParser>();
            _loggerMock = new Mock<ILogger<CaraguatatubaScraper>>();

            _scraper = new CaraguatatubaScraper(
                _fetcherMock.Object,
                _parserMock.Object,
                _loggerMock.Object
            );
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
                        var s when s.Contains("a::text") => "Titulo Teste",
                        var s when s.Contains("href") => "http://link.com",
                        var s when s.Contains("news-text") => "Resumo",
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
            var result = await _scraper.ProcessScraperAsync("teste", 1);

            // Assert
            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Titulo Teste");
            result[0].Link.Should().Be("http://link.com");
        }

        [Test]
        public async Task ProcessScraperAsync_WhenNoPagination_ShouldReturnDefaultSinglePage()
        {
            SetupFetcher();
            SetupPagination(); // vazio
            SetupNewsExtraction();

            var result = await _scraper.ProcessScraperAsync("teste", 1);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldCallFetcherForPagination()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3", "4", "5");
            SetupNewsExtraction(NewsHtml);
            SetupDefaultMapping();

            await _scraper.ProcessScraperAsync("teste", 2);

            _fetcherMock.Verify(
                x => x.GetStringAsync(It.Is<string>(url => url.Contains("page"))),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldRespectDepthLimit()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3", "4", "5");
            SetupNewsExtraction();

            await _scraper.ProcessScraperAsync("teste", 1);

            _fetcherMock.Verify(
                x => x.GetStringAsync(It.Is<string>(url => url.Contains("page"))),
                Times.AtMost(1));
        }

        [Test]
        public async Task ProcessScraperAsync_WhenNoNews_ShouldReturnEmptyList()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNewsExtraction();

            var result = await _scraper.ProcessScraperAsync("teste", 1);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldUseFallbackValues_WhenMissingFields()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3");

            SetupNewsExtraction(NewsHtml);

            _parserMock
                .Setup(x => x.QueryText(NewsHtml, It.IsAny<string>()))
                .Returns((string _, string selector) =>
                {
                    return selector.Contains("created-at") ? "01/01/2024" : null;
                });

            var result = await _scraper.ProcessScraperAsync("teste", 1);

            result[0].Title.Should().Be("No title");
            result[0].Link.Should().Be("No link");
        }

        [Test]
        public async Task ProcessScraperAsync_WhenInvalidDate_ShouldNotThrow()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNewsExtraction(NewsHtml);

            _parserMock
                .Setup(x => x.QueryText(NewsHtml, It.IsAny<string>()))
                .Returns("invalid-date");

            Func<Task> act = () => _scraper.ProcessScraperAsync("teste", 1);

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task ProcessScraperAsync_WhenFetcherFails_ShouldThrowException()
        {
            _fetcherMock
                .Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro"));

            Func<Task> act = () => _scraper.ProcessScraperAsync("teste", 1);

            await act
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Erro");
        }
    }
}