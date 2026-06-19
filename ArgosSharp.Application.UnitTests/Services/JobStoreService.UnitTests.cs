using ArgosSharp.Application.Interfaces.Persistence;
using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Infrastructure.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class JobStoreTests
    {
        private Mock<IJobPersistence> _persistenceMock;
        private IJobRepository _store;
        private IJobFactory _jobFactory;

        [SetUp]
        public void Setup()
        {
            _persistenceMock = new Mock<IJobPersistence>();
            _store = new JobRepository();

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
            await _store.AddAsync(job);

            // Assert
            job.JobId.Should().BeGreaterThan(0);
            _persistenceMock.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Job>>()), Times.Once);
        }

        [Test]
        public async Task GetJobAsync_ShouldReturnJob_WhenExists()
        {
            // Arrange
            var job = _jobFactory.Create("teste", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1));
            await _store.AddAsync(job);

            // Act
            var result = await _store.GetAsync(job.JobHash);

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
            await _store.AddAsync(job1);
            await _store.AddAsync(job2);

            // Act
            var jobs = await _store.GetAllAsync();

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
            await _store.AddAsync(job);

            job.Status = JobStatusEnum.Completed;

            // Act
            await _store.UpdateAsync(job);

            // Assert
            var result = await _store.GetAsync(job.JobHash);
            result!.Status.Should().Be(JobStatusEnum.Completed);
            _persistenceMock.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Job>>()), Times.Exactly(2));
        }
    }
}
