using ArgosSharp.Application.Services.JobStore;
using ArgosSharp.Application.UseCase.Scraper;
using Moq;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobProcessorServiceUnitTests
    {
        private MockRepository mockRepository;
        private Mock<IScraperProcessor> scraperProcessorMock;
        private Mock<IJobStore> jobStoreMock;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            scraperProcessorMock = mockRepository.Create<IScraperProcessor>();
            jobStoreMock = mockRepository.Create<IJobStore>();
        }
    }
}
