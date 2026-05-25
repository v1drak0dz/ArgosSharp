using Moq;

namespace ArgosSharp.Infrastructure.UnitTests.Http.Fetcher
{
    public class HttpClientFetcher
    {
        private Mock<HttpClient> _httpClientMock;
        private MockRepository _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _httpClientMock = new Mock<HttpClient>(MockBehavior.Strict);
        }

        [Test]
        public void GetStringAsync_WhenUrlIsNull_ReturnError()
        {

        }
    }
}
