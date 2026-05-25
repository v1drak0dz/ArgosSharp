using ArgosSharp.Application.StrategiesContext.Scraper;
using Moq;

namespace ArgosSharp.Application.UnitTests.UseCase
{
    public class ScraperProcessorUnitTest
    {
        private MockRepository mockRepository;
        private Mock<IScraperStrategyContext> scraperStrategyContextMock;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            scraperStrategyContextMock = mockRepository.Create<IScraperStrategyContext>();
        }
    }
}
