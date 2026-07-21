using FluentAssertions;
using Moq;
using ArgosSharp.Application.Services.JobProcessor;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Factories.JobFactory;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobProcessorServiceTests
    {
        private Mock<IJobUnitOfWork> _jobUnitOfWork;
        private Mock<IScraperProcessor> _scraperProcessorMock;
        private MockRepository _mockRepository;
        private JobProcessorService _service;
        private JobFactory _jobFactory;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            SetupJobStore();
            _scraperProcessorMock = _mockRepository.Create<IScraperProcessor>();
            _service = new JobProcessorService(_scraperProcessorMock.Object, _jobUnitOfWork.Object);
            _jobFactory = new JobFactory();
        }

        private void SetupJobStore()
        {
            _jobUnitOfWork = _mockRepository.Create<IJobUnitOfWork>();
            _jobUnitOfWork.Setup(x => x.UpdateJobAsync(It.IsAny<Job>())).Returns(Task.CompletedTask);
            _jobUnitOfWork.Setup(x => x.UpdateJobStatus(It.IsAny<Job>(), It.IsAny<JobStatusEnum>())).Returns(Task.CompletedTask);
        }

        [Test]
        public async Task ProcessJobAsync_WhenScraperSucceeds_ShouldSetStatusToCompleted()
        {
            // Arrange
            var job = _jobFactory.Create(
                searchTerm: "teste",
                depth: 2,
                sites: ["caraguatatuba", "ubatuba"]
            );

            var fakeData = new List<Noticia> { CreateNoticia("News 1"), CreateNoticia("News 2") };

            _scraperProcessorMock
                .Setup(x => x.GetNoticias(job.SearchTerm, job.Parameters.Depth, job.Parameters.Sites))
                .ReturnsAsync(fakeData);
            _jobUnitOfWork
                .Setup(x => x.UpdateJobStatus(job, JobStatusEnum.Processing))
                .Callback(() => job.Status = JobStatusEnum.Processing)
                .Returns(Task.CompletedTask);
            _jobUnitOfWork
                .Setup(x => x.UpdateJobStatus(job, JobStatusEnum.Completed))
                .Callback(() => job.Status = JobStatusEnum.Completed)
                .Returns(Task.CompletedTask);

            // Act
            await _service.ProcessJobAsync(job);

            // Assert
            job.Status.Should().Be(JobStatusEnum.Completed);
            job.Data.Should().BeEquivalentTo(fakeData);
            _jobUnitOfWork.Verify(x => x.UpdateJobStatus(job, JobStatusEnum.Processing), Times.Exactly(1));
            _jobUnitOfWork.Verify(x => x.UpdateJobStatus(job, JobStatusEnum.Completed), Times.Exactly(1));
        }

        [Test]
        public async Task ProcessJobAsync_WhenScraperThrowsException_ShouldSetStatusToFailed()
        {
            // Arrange
            var job = _jobFactory.Create(
                searchTerm: "teste",
                depth: 1,
                sites: ["caraguatatuba"]
            );

            _scraperProcessorMock
                .Setup(x => x.GetNoticias(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<string>>()))
                .ThrowsAsync(new InvalidOperationException("Simulated Error"));
            _jobUnitOfWork
                .Setup(x => x.UpdateJobStatus(job, JobStatusEnum.Processing))
                .Callback(() => job.Status = JobStatusEnum.Processing)
                .Returns(Task.CompletedTask);
            _jobUnitOfWork
                .Setup(x => x.UpdateJobStatus(job, JobStatusEnum.Failed))
                .Callback(() => job.Status = JobStatusEnum.Failed)
                .Returns(Task.CompletedTask);

            // Act
            await _service.ProcessJobAsync(job);

            // Assert
            job.Status.Should().Be(JobStatusEnum.Failed);
            job.Error.Should().Be("Simulated Error");
            _jobUnitOfWork.Verify(x => x.UpdateJobStatus(job, JobStatusEnum.Processing), Times.Exactly(1));
            _jobUnitOfWork.Verify(x => x.UpdateJobStatus(job, JobStatusEnum.Failed), Times.Exactly(1));
        }

        private static Noticia CreateNoticia(string title, string description = "Some text")
        {
            return new Noticia(title, DateTime.Now, DateTime.Now.Year, "https://example.com", description, "Scraper");
        }
    }
}
