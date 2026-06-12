using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Infrastructure.Strategies.Scrapers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArgosSharp.Infrastructure.UnitTests.Strategies.Scrapers
{
    public class SaoSebastiaoScraperTests
    {
        private Mock<IHttpFetcher> _fetcherMock;
        private Mock<IHtmlParser> _parserMock;
        private Mock<ILogger<SaoSebastiaoScraper>> _loggerMock;

        private SaoSebastiaoScraper _scraper;

        private const string Html = "<html>";
        private const string NewsHtml = "<news>";

        [SetUp]
        public void Setup()
        {
            _fetcherMock = new Mock<IHttpFetcher>();
            _parserMock = new Mock<IHtmlParser>();
            _loggerMock = new Mock<ILogger<SaoSebastiaoScraper>>();

            _scraper = new SaoSebastiaoScraper(
                _loggerMock.Object,
                _fetcherMock.Object,
                _parserMock.Object
            );
        }

        private void SetupFetcher()
        {
            _fetcherMock
                .Setup(x => x.GetStringAsync(It.Is<string>(url =>
                    url.Contains("noticia-lista"))))
                .ReturnsAsync(Html);
        }

        private void SetupPagination(params string[] pages)
        {
            _parserMock
                .Setup(x => x.QueryTexts(Html,
                    It.Is<string>(s => s.Contains("news_paging"))))
                .Returns(pages.ToList());
        }

        private void SetupNews(params string[] news)
        {
            _parserMock
                .Setup(x => x.QueryTexts(Html,
                    It.Is<string>(s => s.Contains("page-content"))))
                .Returns(news.ToList());
        }

        private void SetupMapping()
        {
            _parserMock
                .Setup(x => x.QueryText(NewsHtml, It.IsAny<string>()))
                .Returns((string _, string selector) =>
                {
                    return selector switch
                    {
                        var s when s.Contains("notice-date") => "01/01/2024",
                        var s when s.Contains("a::text") => "Titulo Teste",
                        var s when s.Contains("href") => "http://link.com",
                        _ => null
                    };
                });
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldReturnFormattedNoticias()
        {
            // Arrange
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNews(NewsHtml);
            SetupMapping();

            // Act
            var result = await _scraper.ProcessScraperAsync("teste", 1);

            // Assert
            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Titulo Teste");
        }

        [Test]
        public async Task ProcessScraperAsync_WhenNoPagination_ShouldReturnEmpty()
        {
            SetupFetcher();
            SetupPagination();
            SetupNews();

            var result = await _scraper.ProcessScraperAsync("teste", 1);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldCallPaginationUrls()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNews(NewsHtml);
            SetupMapping();

            await _scraper.ProcessScraperAsync("teste", 2);

            _fetcherMock.Verify(x =>
                x.GetStringAsync(It.Is<string>(url => url.Contains("&pg="))),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task ProcessScraperAsync_WhenNoNews_ShouldReturnEmptyList()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNews();

            var result = await _scraper.ProcessScraperAsync("teste", 1);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task ProcessScraperAsync_ShouldUseFallbackValues_WhenMissingFields()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNews(NewsHtml);

            _parserMock
                .Setup(x => x.QueryText(NewsHtml, It.IsAny<string>()))
                .Returns((string _, string selector) =>
                {
                    return selector.Contains("notice-date") ? "01/01/2024" : null;
                });

            var result = await _scraper.ProcessScraperAsync("teste", 1);

            result[0].Title.Should().Be("No title");
        }

        [Test]
        public async Task ProcessScraperAsync_WhenInvalidDate_ShouldNotThrow()
        {
            SetupFetcher();
            SetupPagination("1", "2", "3");
            SetupNews(NewsHtml);

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
