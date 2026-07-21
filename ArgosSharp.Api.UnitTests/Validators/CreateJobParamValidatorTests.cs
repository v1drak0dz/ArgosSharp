using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Api.Validators;
using FluentAssertions;

namespace ArgosSharp.Api.UnitTests.Validators
{
    [TestFixture]
    public class CreateJobParamValidatorTests
    {
        private CreateJobParametersValidator _validator;

        [SetUp]
        public void Setup()
            => _validator = new CreateJobParametersValidator();

        [Test]
        public void DepthRule_WhenDepthIsZero_ShouldHaveValidationError()
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = 0, Sites = ["site1"] };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.First().ErrorMessage.Should().Be("Depth must be greater than 0.");
        }

        [Test]
        public void DepthRule_WhenDepthIsLowerThanZero_ShouldHaveValidationError()
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = -1, Sites = ["site1"] };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.First().ErrorMessage.Should().Be("Depth must be greater than 0.");
        }

        [Test]
        public void SitesRule_WhenSitesListIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = 1, Sites = [] };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.First().ErrorMessage.Should().Be("Sites list cannot be empty.");
        }

        [TestCaseSource(nameof(InvalidSitesCases))]
        public void SitesRule_WhenSitesListContainsEmptyString_ShouldHaveValidationError(List<string> sites)
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = 1, Sites = [..sites] };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(error
                => error.PropertyName == nameof(CreateJobParametersRequest.Sites)
                && error.ErrorMessage == "Sites list cannot contain empty or whitespace strings.");
        }

        [Test]
        public void SitesRule_WhenSitesIsNull_ShouldHaveValidationError()
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = 1, Sites = null };
         
            // Act
            var result = _validator.Validate(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(error
                => error.PropertyName == nameof(CreateJobParametersRequest.Sites)
                && error.ErrorMessage == "Sites list cannot be null.");
        }

        [Test]
        public void SitesAndDepthRules_WhenBothAreInvalid_ShouldHaveValidationErrors()
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = 0, Sites = ["site1", " "] };
            
            // Act
            var result = _validator.Validate(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(error
                => error.PropertyName == nameof(CreateJobParametersRequest.Depth)
                && error.ErrorMessage == "Depth must be greater than 0.");
            result.Errors.Should().ContainSingle(error
                => error.PropertyName == nameof(CreateJobParametersRequest.Sites)
                && error.ErrorMessage == "Sites list cannot contain empty or whitespace strings.");
        }

        [Test]
        public void ParamRules_WhenAllRulesValid_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = 1, Sites = ["site1", "site2"] };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        private static IEnumerable<TestCaseData> InvalidSitesCases()
        {
            yield return new TestCaseData(
                new List<string> { "site1", " " })
                .SetName("Sites containing whitespace");

            yield return new TestCaseData(
                new List<string> { "site1", "" })
                .SetName("Sites containing empty string");

            yield return new TestCaseData(
                new List<string> { " " })
                .SetName("Sites containing only whitespace");

            yield return new TestCaseData(
                new List<string> { "" })
                .SetName("Sites containing only empty string");

        }
    }
}
