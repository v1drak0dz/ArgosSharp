using FluentAssertions;
using ArgosSharp.Application.Services.PersistenceService;
using ArgosSharp.Domain.ValueObjects;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.Factories.JobFactory;

namespace ArgosSharp.Application.UnitTests.Services
{
    public class PersistenceServiceTests
    {
        private string _testFilePath;
        private PersistenceService _service;
        private JobFactory _jobFactory;

        [SetUp]
        public void Setup()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
            _service = new PersistenceService(_testFilePath);
            _jobFactory = new JobFactory();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [Test]
        public async Task SaveAsync_ShouldCreateFile_WithSerializedJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                _jobFactory.Create("job2", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1))
            };

            // Act
            await _service.SaveAsync(jobs);

            // Assert
            File.Exists(_testFilePath).Should().BeTrue();
            var content = await File.ReadAllTextAsync(_testFilePath);
            content.Should().Contain("job2");
        }

        [Test]
        public async Task LoadAsync_ShouldReturnJobs_WhenFileExists()
        {
            // Arrange
            var jobs = new List<Job>
            {
                _jobFactory.Create("job2", new JobParameters([ScraperSourceEnum.Caraguatatuba], 1))
            };
            await _service.SaveAsync(jobs);

            // Act
            var loaded = await _service.LoadAsync();

            // Assert
            loaded.Should().HaveCount(1);
            loaded[0].SearchTerm.Should().Be("job2");
        }

        [Test]
        public async Task LoadAsync_ShouldReturnEmptyList_WhenFileDoesNotExist()
        {
            // Act
            var loaded = await _service.LoadAsync();

            // Assert
            loaded.Should().BeEmpty();
        }
    }
}
