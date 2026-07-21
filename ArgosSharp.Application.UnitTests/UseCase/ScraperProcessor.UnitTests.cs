using FluentAssertions;
using Moq;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Application.StrategiesContext.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UnitTests.UseCase
{
    public class ScraperProcessorTests
    {
        private Mock<IScraperStrategyContext> _contextMock;
        private ScraperProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<IScraperStrategyContext>();
            _processor = new ScraperProcessor(_contextMock.Object);
        }

        [Test]
        public void GetNoticias_ShouldThrow_WhenSearchTermIsNullOrEmpty()
        {
            Func<Task> act = async () => await _processor.GetNoticias("", 1, [ScraperSourceEnum.Caraguatatuba]);
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public void GetNoticias_ShouldThrow_WhenDepthIsNegative()
        {
            Func<Task> act = async () => await _processor.GetNoticias("teste", -1, [ScraperSourceEnum.Caraguatatuba]);
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task GetNoticias_ShouldCallContext_ForEachSource()
        {
            // Arrange
            var sources = new[] { ScraperSourceEnum.Caraguatatuba, ScraperSourceEnum.Ubatuba };

            _contextMock.Setup(c => c.GetNoticiasBySourceAsync(It.IsAny<ScraperSourceEnum>(), It.IsAny<string>(), It.IsAny<int>()))
                        .ReturnsAsync((ScraperSourceEnum src, string term, int depth) =>
                            new List<Noticia>
                            {
                                new($"Fake result for {src}", DateTime.Now, DateTime.Now.Year, "Some link", "Some abstract", "Test")
                            });

            // Act
            var result = await _processor.GetNoticias("teste", 1, sources);

            // Assert
            result.Should().HaveCount(2);
            result[0].Title.Should().Contain("Caraguatatuba");
            result[1].Title.Should().Contain("Ubatuba");

            _contextMock.Verify(c => c.GetNoticiasBySourceAsync(ScraperSourceEnum.Caraguatatuba, "teste", 1), Times.Once);
            _contextMock.Verify(c => c.GetNoticiasBySourceAsync(ScraperSourceEnum.Ubatuba, "teste", 1), Times.Once);
        }

        [Test]
        public async Task GetNoticias_ShouldReturnEmptyList_WhenNoSources()
        {
            var result = await _processor.GetNoticias("teste", 1, Array.Empty<ScraperSourceEnum>());
            result.Should().BeEmpty();
        }
    }
}
