using Moq;

using ArgosSharp.Application.Interfaces.Fetcher;
using ArgosSharp.Application.Interfaces.Parser;
using Microsoft.Extensions.Logging;

namespace ArgosSharp.Infrastructure.UnitTests.Strategies.Scrapers
{
    public class CaraguatatubaScraperUnitTests
    {
        private Mock<IHttpFetcher> _httpFetcherMock;
        private Mock<IHtmlParser> _htmlParserMock;
        private Mock<ILogger> _loggerMock;
        private MockRepository _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _httpFetcherMock = _mockRepository.Create<IHttpFetcher>(MockBehavior.Strict);
            _htmlParserMock = _mockRepository.Create<IHtmlParser>(MockBehavior.Strict);
            _loggerMock = _mockRepository.Create<ILogger>(MockBehavior.Strict);

        }

        [Test]
        public void ProcessScraperAsync_WhenSearchTermIsEmpty_ThrowError()
        {
            
        }
    }
}
