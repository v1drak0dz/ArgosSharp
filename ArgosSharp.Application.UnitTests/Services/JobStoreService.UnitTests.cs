using ArgosSharp.Application.Services.PersistenceService;
using Moq;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobStoreServiceUnitTests
    {
        private MockRepository mockRepository;
        private Mock<IPersistenceService> persistenceServiceMock;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            persistenceServiceMock = mockRepository.Create<IPersistenceService>();
        }
    }
}
