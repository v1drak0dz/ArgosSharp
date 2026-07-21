using ArgosSharp.Api.Controllers;
using ArgosSharp.Api.DTOs.Job;
using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Api.Mappers.JobMapper;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ArgoSharp.Api.UnitTests.Controllers
{
    public class JobControllerTests
    {
        private MockRepository mockRepository;
        private Mock<IJobQueue> jobQueueMock;
        private Mock<IJobMapper> jobMapperMock;
        private JobsController jobsController;
        private CreateJobRequest sampleJob;
        private Job jobObject;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            jobQueueMock = mockRepository.Create<IJobQueue>();
            jobMapperMock = mockRepository.Create<IJobMapper>();

            sampleJob = new CreateJobRequest
            {
                SearchTerm = "Test",
                Parameters = new CreateJobParametersRequest { Depth = 1, Sites = ["Caraguatatuba"] }
            };
            jobObject = new Job("Test", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1), JobStatusEnum.Created);

            jobMapperMock.Setup(x => x.JobFromRequest(It.Is<CreateJobRequest>(x => x.Equals(sampleJob)))).Returns(jobObject);
            jobsController = new JobsController(jobQueueMock.Object, jobMapperMock.Object);
        }

        [Test]
        public async Task CreateJobAsync_WhenValidJob_ShouldReturnOk()
        {
            // Arrange
            jobQueueMock.Setup(x => x.EnqueueAsync(It.IsAny<Job>())).Returns(Task.CompletedTask);

            // Act
            var result = await jobsController.CreateJobAsync(sampleJob);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as JobResponseDTO;

            response.Should().NotBeNull();
            response!.JobId.Should().Be(jobObject.JobId);
        }

        [Test]
        public async Task CreateJobAsync_WhenInvalidJob_ShouldReturnNotFound()
        {
            // Arrange
            jobQueueMock.Setup(x => x.EnqueueAsync(It.IsAny<Job>())).Throws(new Exception());

            // Act
            var result = await jobsController.CreateJobAsync(sampleJob);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
