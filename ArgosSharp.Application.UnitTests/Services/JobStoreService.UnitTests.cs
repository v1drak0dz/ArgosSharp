using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ArgosSharp.Application.Services.JobStore;
using ArgosSharp.Application.Services.PersistenceService;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Factories.JobFactory;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobStoreTests
    {
        private Mock<IPersistenceService> _persistenceMock;
        private JobStore _store;
        private IJobFactory _jobFactory;

        [SetUp]
        public void Setup()
        {
            _persistenceMock = new Mock<IPersistenceService>();
            _store = new JobStore(_persistenceMock.Object);

            _persistenceMock.Setup(x => x.SaveAsync(It.IsAny<IEnumerable<Job>>()))
                            .Returns(Task.CompletedTask);

            _jobFactory = new JobFactory();
        }

        [Test]
        public async Task AddJobAsync_ShouldAssignJobId_AndPersist()
        {
            // Arrange
            var job = _jobFactory.Create("teste", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));

            // Act
            await _store.AddJobAsync(job);

            // Assert
            job.JobId.Should().BeGreaterThan(0);
            _persistenceMock.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Job>>()), Times.Once);
        }

        [Test]
        public async Task GetJobAsync_ShouldReturnJob_WhenExists()
        {
            // Arrange
            var job = _jobFactory.Create("teste", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));
            await _store.AddJobAsync(job);

            // Act
            var result = await _store.GetJobAsync(job.JobHash);

            // Assert
            result.Should().NotBeNull();
            result!.SearchTerm.Should().Be("teste");
        }

        [Test]
        public async Task GetAllJobsAsync_ShouldReturnAllJobs()
        {
            // Arrange
            var job1 = _jobFactory.Create("job1", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));
            var job2 = _jobFactory.Create("job2", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));
            await _store.AddJobAsync(job1);
            await _store.AddJobAsync(job2);

            // Act
            var jobs = await _store.GetAllJobsAsync();

            // Assert
            jobs.Should().HaveCount(2);
            jobs.Should().Contain(j => j.SearchTerm == "job1");
            jobs.Should().Contain(j => j.SearchTerm == "job2");
        }

        [Test]
        public async Task UpdateAsync_ShouldReplaceJob_AndPersist()
        {
            // Arrange
            var job = _jobFactory.Create("job2", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));
            await _store.AddJobAsync(job);

            job.Status = JobStatusEnum.Completed;

            // Act
            await _store.UpdateAsync(job);

            // Assert
            var result = await _store.GetJobAsync(job.JobHash);
            result!.Status.Should().Be(JobStatusEnum.Completed);
            _persistenceMock.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Job>>()), Times.Exactly(2));
        }

        [Test]
        public async Task InitializeAsync_ShouldLoadJobs_AndMarkProcessingAsFailed()
        {
            // Arrange
            var job1 = _jobFactory.Create("job1", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));
            job1.Status = JobStatusEnum.Completed;

            var job2 = _jobFactory.Create("job2", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));
            job2.Status = JobStatusEnum.Processing;

            var jobs = new List<Job> { job1, job2 };

            _persistenceMock.Setup(x => x.LoadAsync()).ReturnsAsync(jobs);

            // Act
            await _store.InitializeAsync();

            // Assert
            var allJobs = await _store.GetAllJobsAsync();
            allJobs.Should().HaveCount(2);
            allJobs.Should().Contain(j => j.Status == JobStatusEnum.Failed && j.Error == "Application interrupted");
            allJobs.Should().Contain(j => j.Status == JobStatusEnum.Completed);
        }
    }
}
