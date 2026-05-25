using Moq;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobQueueServiceUnitTests
    {
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }
    }
}
