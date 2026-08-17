using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobExecutionFactory.cs;
using ArgosSharp.Domain.ValueObjects;
using FluentAssertions;

namespace ArgosSharp.Domain.UnitTests.Factories
{
    [TestFixture]
    internal class JobFactoryUnitTests
    {
        private IJobExecutionFactory JobExecutionFactory;
        private const int JobId = 1;

        [Test]
        public void Create_WhenValidParameters_ShouldReturnJobExecution()
        {
            // Arrange
            JobExecutionFactory = new JobExecutionFactory();
            var parameters = new JobParameters("Test", ["jobId"], 1);

            // Act
            var execution = JobExecutionFactory.Create(JobId, parameters);

            // Assert
            execution.Should().NotBeNull();
            execution.JobId.Should().Be(JobId);
            execution.Parameters.Should().Be(parameters);
            execution.JobStatus.Should().Be(JobStatusEnum.Created);
            execution.StartedAt.Should().BeNull();
            execution.FinishedAt.Should().BeNull();
            execution.Error.Should().BeNull();
        }

        [TestCase("", TestName = "Create_WhenQueryIsEmpty_ShouldThrowException")]
        [TestCase(null, TestName = "Create_WhenQueryIsNull_ShouldThrowException")]
        public void Create_WhenInvalidQuery_ShouldThrowException(string? invalidQuery)
        {
            // Arrange
            JobExecutionFactory = new JobExecutionFactory();
            var parameters = new JobParameters(invalidQuery, ["jobId"], 1);

            // Act
            var execution = JobExecutionFactory.Create(JobId, parameters);

            // Assert
            execution.Should().NotBeNull();
            execution.JobId.Should().Be(JobId);
            execution.Parameters.Should().Be(parameters);
        }

        [TestCase(-1, TestName = "Create_WhenDepthIsNegative_ShouldThrowException")]
        [TestCase(0, TestName = "Create_WhenDepthIsZero_ShouldThrowException")]
        public void Create_WhenDepthIsInvalid_ShouldThrowException(int invalidDepth)
        {
            // Arrange
            JobExecutionFactory = new JobExecutionFactory();
            var parameters = new JobParameters("Tests", ["jobId"], invalidDepth);

            // Act
            var execution = JobExecutionFactory.Create(JobId, parameters);

            // Assert
            execution.Should().NotBeNull();
            execution.JobId.Should().Be(JobId);
            execution.Parameters.Should().Be(parameters);
        }
    }
}
