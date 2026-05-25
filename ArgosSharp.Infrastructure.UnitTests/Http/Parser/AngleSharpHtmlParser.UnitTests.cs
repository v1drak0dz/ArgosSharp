using Moq;

namespace ArgosSharp.Infrastructure.UnitTests.Http.Parser
{
    public class AngleSharpHtmlParser
    {
        private MockRepository _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
        }
    }
}
