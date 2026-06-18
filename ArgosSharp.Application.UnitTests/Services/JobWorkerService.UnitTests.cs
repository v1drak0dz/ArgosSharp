using Moq;
using Microsoft.Extensions.DependencyInjection;
using ArgosSharp.Application.Services.JobWorker;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Application.Services.JobProcessor;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Domain.Enums;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobWorkerTests
    {
        private JobFactory _jobFactory;

        [SetUp]
        public void SetUp()
        {
            _jobFactory = new JobFactory();
        }

        [Test]
        public async Task ExecuteAsync_ShouldProcessJob_FromQueue()
        {
            // Arrange
            var job = _jobFactory.Create("job2", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));

            var jobQueueMock = new Mock<IJobQueue>();
            jobQueueMock.Setup(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(job);

            var processorMock = new Mock<IJobProcessorService>();
            processorMock.Setup(p => p.ProcessJobAsync(job))
                         .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IJobProcessorService)))
                               .Returns(processorMock.Object);

            var scopeMock = new Mock<IServiceScope>();
            scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            scopeFactoryMock.Setup(sf => sf.CreateScope()).Returns(scopeMock.Object);

            var worker = new JobWorker(jobQueueMock.Object, scopeFactoryMock.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await worker.StartAsync(cts.Token);

            // Assert
            processorMock.Verify(p => p.ProcessJobAsync(job), Times.AtLeastOnce);
        }
    }
}
