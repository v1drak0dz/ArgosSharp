using ArgosSharp.Api.Controllers;
using ArgosSharp.Api.DTOs.Job;
using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Application.UseCase.CreateJob;
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
        private Mock<ICreateJobUseCase> createJobMock;
        private JobsController jobsController;
        private Job jobObject;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            createJobMock = mockRepository.Create<ICreateJobUseCase>();

            jobObject = new Job("Test", new JobParameters(["caraguatatuba"], 1), JobStatusEnum.Created);
            createJobMock.Setup(x => x.CreateJob("Test", new List<string> { "Caraguatatuba" }, 1)).ReturnsAsync(jobObject);

            jobsController = new JobsController(createJobMock.Object);
        }

        [Test]
        public async Task CreateJobAsync_WhenValidJob_ShouldReturnOk()
        {
            // Arrange
            var sampleJob = new CreateJobRequest
            {
                SearchTerm = "Test",
                Parameters = new CreateJobParametersRequest { Depth = 1, Sites = ["Caraguatatuba"] }
            };

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
            var sampleJob = new CreateJobRequest
            {
                SearchTerm = "Test",
                Parameters = new CreateJobParametersRequest { Depth = 1, Sites = ["NotValidOption"] }
            };

            // Act
            var result = await jobsController.CreateJobAsync(sampleJob);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
