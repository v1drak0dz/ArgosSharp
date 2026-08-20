using ArgosSharp.Application.Services.JobQueue;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobQueueServiceUnitTests
    {
        private JobQueue _queue;
        private IJobFactory _jobFactory;
        private CancellationTokenSource _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _queue = new JobQueue();
            _jobFactory = new JobFactory();
            _cancellationToken = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cancellationToken.Dispose();
        }

        [Test]
        public async Task EnqueueAsync_ShouldAddJob()
        {
            // Arrange
            var job = _jobFactory.Create
            (
                searchTerm: "Teste",
                depth: 1,
                sites: ["caraguatatuba"]
            );

            // Act
            await _queue.EnqueueAsync(job);
            var dequeue = await _queue.DequeueAsync(_cancellationToken.Token);

            // Assert
            dequeue.Should().BeSameAs(job);
        }

        [Test]
        public async Task DequeueAsync_ShouldWaitUntilJobIsEnqueue()
        {
            // Arrange
            var job = _jobFactory.Create
            (
                searchTerm: "Teste",
                depth: 1,
                sites: ["caraguatatuba"]
            );

            // Act
            var dequeueTask = _queue.DequeueAsync(_cancellationToken.Token);

            await Task.Delay(180);
            await _queue.EnqueueAsync(job);

            var dequeue = await dequeueTask;

            // Assert
            dequeue.Should().BeSameAs(job);
        }

        [Test]
        public void DequeueAsync_ShouldThrow_WhenCancelled()
        {
            // Arrange
            _cancellationToken.Cancel();

            // Act
            Func<Task> act = async () => await _queue.DequeueAsync(_cancellationToken.Token);

            // Assert
            act.Should().ThrowAsync<TaskCanceledException>();
        }
    }
}
