using Moq;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class ScraperStrategyContextUnitTests
    {
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }
    }
}
