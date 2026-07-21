using ArgosSharp.Application.Interfaces.UnitOfWork;
using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Application.UseCase.CreateJob;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ArgosSharp.Application.UnitTests.UseCase
{
    public class CreateJobUseCaseTests
    {
        private MockRepository MockRepository { get; set; }
        private Mock<IJobFactory> MockJobFactory { get; set; }
        private Mock<IJobUnitOfWork> MockUnitOfWork { get; set; }
        private Mock<IJobQueue> MockJobQueue { get; set; } 
        private CreateJobUseCase CreateJobUseCase { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
            MockJobFactory = MockRepository.Create<IJobFactory>();
            MockUnitOfWork = MockRepository.Create<IJobUnitOfWork>();
            MockJobQueue = MockRepository.Create<IJobQueue>();
            
            CreateJobUseCase = new CreateJobUseCase(MockJobFactory.Object, MockUnitOfWork.Object, MockJobQueue.Object);
        }

        [Test]
        public async Task CreateJob_WhenValidJob_ShouldReturnJob()
        {
            // Arrange
            var sampleJob = new Job("Test", new JobParameters(depth: 1, sites: ["caraguatatuba"]), JobStatusEnum.Created);
            MockJobFactory.Setup(x => x.Create("Test", new List<string> { "caraguatatuba" }, 1)).Returns(sampleJob);
            MockUnitOfWork.Setup(x => x.AddJobAsync(sampleJob)).Returns(Task.CompletedTask);
            MockJobQueue.Setup(x => x.EnqueueAsync(sampleJob)).Returns(Task.CompletedTask);
            MockUnitOfWork
                .Setup(x => x.UpdateJobStatus(sampleJob, JobStatusEnum.Enqueued))
                .Callback(() => sampleJob.Status = JobStatusEnum.Enqueued)
                .Returns(Task.CompletedTask);

            // Act
            var result = await CreateJobUseCase.CreateJob("Test", ["caraguatatuba"], 1);

            // Verify
            result.Should().Be(sampleJob);
        }

        [Test]
        public async Task CreateJob_WhenFailedToAddJob_ShouldThrowError()
        {
            // Arrange
            var sampleJob = new Job("Test", new JobParameters(depth: 1, sites: ["invalid"]), JobStatusEnum.Created);
            MockJobFactory.Setup(x => x.Create("Test", new List<string> { "invalid" }, 1)).Returns(sampleJob);
            MockUnitOfWork.Setup(x => x.AddJobAsync(sampleJob)).Throws(new Exception());
            MockJobQueue.Setup(x => x.EnqueueAsync(sampleJob)).Returns(Task.CompletedTask);
            MockUnitOfWork
                .Setup(x => x.UpdateJobStatus(sampleJob, JobStatusEnum.Enqueued))
                .Callback(() => sampleJob.Status = JobStatusEnum.Enqueued)
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await CreateJobUseCase.CreateJob("Test", ["invalid"], 1);
            
            // Verify
            await act.Should().ThrowAsync<Exception>();
        }

        [Test]
        public async Task CreateJob_WhenFailedToEnqueueJob_ShouldThrowError()
        {
            // Arrange
            var sampleJob = new Job("Test", new JobParameters(depth: 1, sites: ["invalid"]), JobStatusEnum.Created);
            MockJobFactory.Setup(x => x.Create("Test", new List<string> { "invalid" }, 1)).Returns(sampleJob);
            MockUnitOfWork.Setup(x => x.AddJobAsync(sampleJob)).Returns(Task.CompletedTask);
            MockJobQueue.Setup(x => x.EnqueueAsync(sampleJob)).Throws(new Exception());
            MockUnitOfWork
                .Setup(x => x.UpdateJobStatus(sampleJob, JobStatusEnum.Enqueued))
                .Callback(() => sampleJob.Status = JobStatusEnum.Enqueued)
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await CreateJobUseCase.CreateJob("Test", ["invalid"], 1);

            // Verify
            await act.Should().ThrowAsync<Exception>();
        }

        [Test]
        public async Task CreateJob_WhenFailedToUpdateJob_ShouldThrowError()
        {
            // Arrange
            var sampleJob = new Job("Test", new JobParameters(depth: 1, sites: ["invalid"]), JobStatusEnum.Created);
            MockJobFactory.Setup(x => x.Create("Test", new List<string> { "invalid" }, 1)).Returns(sampleJob);
            MockUnitOfWork.Setup(x => x.AddJobAsync(sampleJob)).Returns(Task.CompletedTask);
            MockJobQueue.Setup(x => x.EnqueueAsync(sampleJob)).Returns(Task.CompletedTask);
            MockUnitOfWork
                .Setup(x => x.UpdateJobStatus(sampleJob, JobStatusEnum.Enqueued))
                .Throws(new Exception());

            // Act
            Func<Task> act = async () => await CreateJobUseCase.CreateJob("Test", ["invalid"], 1);

            // Verify
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
