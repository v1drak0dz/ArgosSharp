using ArgosSharp.Application.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using ArgosSharp.Application.Services.JobProcessor;
using ArgosSharp.Application.UseCase.Scraper;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Factories.JobFactory;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobProcessorServiceTests
    {
        private Mock<IJobRepository> _jobStoreMock;
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
            _service = new JobProcessorService(_scraperProcessorMock.Object, _jobStoreMock.Object);
            _jobFactory = new JobFactory();
        }

        private void SetupJobStore()
        {
            _jobStoreMock = _mockRepository.Create<IJobRepository>();
            _jobStoreMock.Setup(x => x.UpdateAsync(It.IsAny<Job>())).Returns(Task.CompletedTask);
        }

        [Test]
        public async Task ProcessJobAsync_WhenScraperSucceeds_ShouldSetStatusToCompleted()
        {
            // Arrange
            var job = _jobFactory.Create(
                searchTerm: "teste",
                parameters: new JobParameters(depth: 2, sites: [ScraperSourceEnum.Caraguatatuba, ScraperSourceEnum.Ubatuba])
            );

            var fakeData = new List<Noticia> { CreateNoticia("News 1"), CreateNoticia("News 2") };

            _scraperProcessorMock
                .Setup(x => x.GetNoticias(job.SearchTerm, job.Parameters.Depth, job.Parameters.Sites))
                .ReturnsAsync(fakeData);

            // Act
            await _service.ProcessJobAsync(job);

            // Assert
            job.Status.Should().Be(JobStatusEnum.Completed);
            job.Data.Should().BeEquivalentTo(fakeData);
            _jobStoreMock.Verify(x => x.UpdateAsync(job), Times.Exactly(2));
        }

        [Test]
        public async Task ProcessJobAsync_WhenScraperThrowsException_ShouldSetStatusToFailed()
        {
            // Arrange
            var job = _jobFactory.Create(
                searchTerm: "teste",
                parameters: new JobParameters(depth: 1, sites: [ScraperSourceEnum.Caraguatatuba])
            );

            _scraperProcessorMock
                .Setup(x => x.GetNoticias(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<ScraperSourceEnum>>()))
                .ThrowsAsync(new InvalidOperationException("Erro simulado"));

            // Act
            await _service.ProcessJobAsync(job);

            // Assert
            job.Status.Should().Be(JobStatusEnum.Failed);
            job.Error.Should().Be("Erro simulado");
            _jobStoreMock.Verify(x => x.UpdateAsync(job), Times.Exactly(2));
        }

        private static Noticia CreateNoticia(string title, string description = "Some text")
        {
            return new Noticia(title, DateTime.Now, DateTime.Now.Year, "https://example.com", description, "Scraper");
        }
    }
}
