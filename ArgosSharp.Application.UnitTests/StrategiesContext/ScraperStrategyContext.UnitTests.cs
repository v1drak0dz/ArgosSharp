using FluentAssertions;
using ArgosSharp.Application.StrategiesContext.Scraper;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using Moq;

namespace ArgosSharp.Application.UnitTests.StrategiesContext
{
    [TestFixture]
    public class ScraperStrategyContextTests
    {
        private const string ValidResultTitle = "Fake result for test";
        private const string SearchTerm = "test";
        private const string ExceptionMessage = "*No Scraper found*";
        private const int Depth = 1;

        private ScraperStrategyContext scraperStrategyContext;
        private Mock<IScraperStrategy> scraperStrategyMock;
        private Mock<IScraperStrategy> validStrategyMock;
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            scraperStrategyMock = mockRepository.Create<IScraperStrategy>();
            scraperStrategyMock.SetupProperty(x => x.Name, "InvalidTestSource");
            scraperStrategyMock.Setup(x =>
                    x.ProcessScraperAsync(It.Is<string>(x => x.Equals(SearchTerm)), It.Is<int>(x => x.Equals(Depth))))
                .Throws(new Exception(ExceptionMessage));

            validStrategyMock = mockRepository.Create<IScraperStrategy>();
            validStrategyMock.SetupProperty(x => x.Name, "ValidTestSource");
            validStrategyMock.Setup(x =>
                    x.ProcessScraperAsync(It.Is<string>(x => x.Equals(SearchTerm)), It.Is<int>(x => x.Equals(Depth))))
                .ReturnsAsync([CreateNoticia(ValidResultTitle)]);
            scraperStrategyContext = new ScraperStrategyContext([scraperStrategyMock.Object, validStrategyMock.Object]);
        }

        [Test]
        public async Task GetNoticiasBySourceAsync_ShouldReturnResults_FromMatchingScraper()
        {
            // Act
            var result = await scraperStrategyContext.GetNoticiasBySourceAsync(
                ScraperSourceEnum.ValidTestSource, SearchTerm, Depth);

            // Assert
            result.Should().NotBeEmpty();
            result[0].Title.Should().Contain(ValidResultTitle);
        }

        [Test]
        public void GetNoticiasBySourceAsync_ShouldThrow_WhenNoScraperFound()
        {
            // Act
            Func<Task> act = async () =>
                await scraperStrategyContext.GetNoticiasBySourceAsync(ScraperSourceEnum.InvalidTestSource, SearchTerm, Depth);

            // Assert
            act.Should().ThrowAsync<Exception>()
               .WithMessage(ExceptionMessage);
        }

        private Noticia CreateNoticia(string title) =>
            new(title, DateTime.Now, DateTime.Now.Year, "some link", "Some abstract", "Source");
    }
}
