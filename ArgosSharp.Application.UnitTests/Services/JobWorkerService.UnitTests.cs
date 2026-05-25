using ArgosSharp.Application.Services.JobQueue;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobWorkerServiceUnitTests
    {
        private MockRepository mockRepository;
        private Mock<IJobQueue> jobQueueMock;
        private Mock<IServiceScopeFactory> serviceScopeFactory;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            jobQueueMock = mockRepository.Create<IJobQueue>();
            serviceScopeFactory = mockRepository.Create<IServiceScopeFactory>();
        }
    }
}
