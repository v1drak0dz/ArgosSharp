using FluentAssertions;
using ArgosSharp.Application.StrategiesContext.Scraper;
using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.Annotations;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.UnitTests.StrategiesContext
{
    [ScraperSourceAnnotation(ScraperSourceEnum.Caraguatatuba)]
    public class FakeScraper : IScraperStrategy
    {
        public string Name => "Test";

        public Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth)
        {
            return Task.FromResult(new List<Noticia>
            {
                new("Fake result for teste", DateTime.Now, DateTime.Now.Year, "Some link", "Some abstract", "Test")
            });
        }
    }

    public class FakeScraperWithoutAttribute : IScraperStrategy
    {
        public string Name => "Unannotated";

        public Task<List<Noticia>> ProcessScraperAsync(string searchTerm, int depth)
        {
            return Task.FromResult(new List<Noticia>
            {
                new("Fake result for no attribute", DateTime.Now, DateTime.Now.Year, "Some link", "Some abstract", "Test")
            });
        }
    }

    public class ScraperStrategyContextTests
    {
        [Test]
        public async Task GetNoticiasBySourceAsync_ShouldReturnResults_FromMatchingScraper()
        {
            // Arrange
            var context = new ScraperStrategyContext();

            // Act
            var result = await context.GetNoticiasBySourceAsync(
                ScraperSourceEnum.Caraguatatuba, "teste", 1);

            // Assert
            result.Should().NotBeEmpty();
            result[0].Title.Should().Contain("Fake result for teste");
        }

        [Test]
        public void GetNoticiasBySourceAsync_ShouldThrow_WhenNoScraperFound()
        {
            // Arrange
            var context = new ScraperStrategyContext();

            // Act
            Func<Task> act = async () =>
                await context.GetNoticiasBySourceAsync(ScraperSourceEnum.TestSource, "teste", 1);

            // Assert
            act.Should().ThrowAsync<Exception>()
               .WithMessage("*No Scraper found*");
        }
    }
}
