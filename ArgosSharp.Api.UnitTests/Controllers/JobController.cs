using ArgosSharp.Api.Controllers;
using ArgosSharp.Api.DTOs.Job;
using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Application.UseCase.CreateJob;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ArgosSharp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class JobControllerTests
    {
        private MockRepository _mockRepository;
        private Mock<ICreateJobUseCase> _createJobUseCaseMock;
        private Mock<IValidator<CreateJobRequest>> _createJobValidatorMock;
        private Mock<IValidator<CreateJobParametersRequest>> _createJobParametersValidatorMock;

        private JobsController _jobsController;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _createJobUseCaseMock = _mockRepository.Create<ICreateJobUseCase>();
            _createJobValidatorMock = _mockRepository.Create<IValidator<CreateJobRequest>>();
            _createJobParametersValidatorMock = _mockRepository.Create<IValidator<CreateJobParametersRequest>>();

            _jobsController = new JobsController(
                _createJobUseCaseMock.Object,
                _createJobValidatorMock.Object,
                _createJobParametersValidatorMock.Object);
        }

        [TearDown]
        public void TearDown()
            => _mockRepository.VerifyAll();

        [Test]
        public async Task CreateJobAsync_WhenRequestIsValid_ShouldReturnOk()
        {
            // Arrange
            var request = CreateValidRequest();

            var createdJob = new Job(
                "Test",
                new JobParameters(["caraguatatuba"], 1),
                JobStatusEnum.Created);

            _createJobValidatorMock
                .Setup(x => x.ValidateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _createJobParametersValidatorMock
                .Setup(x => x.ValidateAsync(
                    request.Parameters!,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _createJobUseCaseMock
                .Setup(x => x.CreateJob(
                    request.SearchTerm!,
                    It.Is<List<string>>(sites =>
                        sites.SequenceEqual(request.Parameters!.Sites)),
                    request.Parameters!.Depth))
                .ReturnsAsync(createdJob);

            // Act
            var result = await _jobsController.CreateJobAsync(request);

            // Assert
            var okResult = result.Result
                .Should()
                .BeOfType<OkObjectResult>()
                .Subject;

            var response = okResult.Value
                .Should()
                .BeOfType<JobResponseDTO>()
                .Subject;

            response.JobId.Should().Be(createdJob.JobId);
        }

        [Test]
        public async Task CreateJobAsync_WhenRequestValidatorFails_ShouldReturnBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();

            var expectedFailure = new ValidationFailure(
                nameof(CreateJobRequest.SearchTerm),
                "SearchTerm is required");

            _createJobValidatorMock
                .Setup(x => x.ValidateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult([expectedFailure]));

            // Act
            var result = await _jobsController.CreateJobAsync(request);

            // Assert
            var failure = GetSingleValidationFailure(result);

            failure.PropertyName
                .Should()
                .Be(nameof(CreateJobRequest.SearchTerm));

            failure.ErrorMessage
                .Should()
                .Be("SearchTerm is required");
        }

        [Test]
        public async Task CreateJobAsync_WhenParametersValidatorFails_ShouldReturnBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();

            var expectedFailure = new ValidationFailure(
                nameof(CreateJobParametersRequest.Depth),
                "Depth must be greater than 0");

            _createJobValidatorMock
                .Setup(x => x.ValidateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _createJobParametersValidatorMock
                .Setup(x => x.ValidateAsync(
                    request.Parameters!,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ValidationResult([expectedFailure]));

            // Act
            var result = await _jobsController.CreateJobAsync(request);

            // Assert
            var failure = GetSingleValidationFailure(result);

            failure.PropertyName
                .Should()
                .Be(nameof(CreateJobParametersRequest.Depth));

            failure.ErrorMessage
                .Should()
                .Be("Depth must be greater than 0");
        }

        private static CreateJobRequest CreateValidRequest()
        {
            return new CreateJobRequest
            {
                SearchTerm = "Test",
                Parameters = new CreateJobParametersRequest
                {
                    Depth = 1,
                    Sites = ["Caraguatatuba"]
                }
            };
        }

        private static ValidationFailure GetSingleValidationFailure(
            ActionResult<JobResponseDTO> result)
        {
            var badRequest = result.Result
                .Should()
                .BeOfType<BadRequestObjectResult>()
                .Subject;

            var failures = badRequest.Value
                .Should()
                .BeAssignableTo<IEnumerable<ValidationFailure>>()
                .Subject;

            return failures
                .Should()
                .ContainSingle()
                .Subject;
        }
    }
}