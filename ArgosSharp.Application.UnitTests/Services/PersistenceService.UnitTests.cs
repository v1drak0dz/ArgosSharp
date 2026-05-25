using Moq;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class PersistenceServiceUnitTests
    {
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }
    }
}
