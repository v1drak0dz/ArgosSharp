using FluentAssertions;
using Moq;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Application.StrategiesContext.Scraper;
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
        public void GetNews_ShouldThrow_WhenSearchTermIsNullOrEmpty()
        {
            Func<Task> act = async () => await _processor.GetNews("", 1, ["Caraguatatuba"]);
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public void GetNews_ShouldThrow_WhenDepthIsNegative()
        {
            Func<Task> act = async () => await _processor.GetNews("teste", -1, ["Caraguatatuba"]);
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task GetNews_ShouldCallContext_ForEachSource()
        {
            // Arrange
            var sources = new[] { "Caraguatatuba", "Ubatuba" };

            _contextMock
                .Setup(c => c.GetNewsBySourceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((string src, string term, int depth) =>
                [
                    CreateFakeResult(src)
                ]);

            // Act
            var result = await _processor.GetNews("teste", 1, sources);

            // Assert
            result.Should().HaveCount(2);
            result[0].Title.Should().Contain("Caraguatatuba");
            result[1].Title.Should().Contain("Ubatuba");

            _contextMock.Verify(c => c.GetNewsBySourceAsync("Caraguatatuba", "teste", 1), Times.Once);
            _contextMock.Verify(c => c.GetNewsBySourceAsync("Ubatuba", "teste", 1), Times.Once);
        }

        [Test]
        public async Task GetNews_ShouldReturnEmptyList_WhenNoSources()
        {
            var result = await _processor.GetNews("teste", 1, []);
            result.Should().BeEmpty();
        }

        private static News CreateFakeResult(string source) =>
            new($"Fake result for {source}", DateTime.Now, DateTime.Now.Year, "Some link", "Some abstract", source);
    }
}
