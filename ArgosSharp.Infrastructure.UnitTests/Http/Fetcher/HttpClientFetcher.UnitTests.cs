using ArgosSharp.Infrastructure.Http.Fetcher;
using Moq;
using FluentAssertions;
using System.Net;
using Moq.Protected;

namespace ArgosSharp.Infrastructure.UnitTests.Http.Fetcher
{
    public class HttpClientFetcherTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandler;
        private MockRepository _mockRepository;
        private HttpClient _httpClient;
        private HttpClientFetcher _fetcher;
        
        private const string TestURL = "https://google.com";
        private const string TestExpected = "Expected Result";

        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _httpMessageHandler = _mockRepository.Create<HttpMessageHandler>(MockBehavior.Strict);

            _httpMessageHandler.Protected().Setup("Dispose", ItExpr.IsAny<bool>());

            _httpClient = new HttpClient(_httpMessageHandler.Object);
            _fetcher = new HttpClientFetcher(_httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
            _mockRepository.VerifyAll();
        }

        [Test]
        public async Task GetStringAsync_WhenValidUrl_ReturnPage()
        {
            // Arrange
            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(TestExpected) });

            // Act
            var result = await _fetcher.GetStringAsync(TestURL);

            // Assert
            result.Should().Be(TestExpected);
        }
    }
}
