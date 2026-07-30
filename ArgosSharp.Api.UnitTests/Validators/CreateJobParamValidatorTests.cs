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

        [TestCase(0, TestName = "Test that the Depth validation rule is triggered when the Depth is zero.")]
        [TestCase(-1, TestName = "Test that the Depth validation rule is triggered when the Depth is lower than 0.")]
        public void DepthRule_WhenDepthIsZero_ShouldHaveValidationError(int depth)
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = depth, Sites = ["site1"] };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.First().ErrorMessage.Should().Be("Depth must be greater than 0.");
        }

        [Test(Description = "Test that the Depth validation rule is not triggered when the Depth is greater than 0.")]
        public void DepthRule_WhenDepthIsGreaterThanZero_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new CreateJobParametersRequest { Depth = 1, Sites = ["site1"] };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Count.Should().Be(0);
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

        [Test(Description = "Test that the Sites validation rule is triggered when the Sites list is null.")]
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

        [Test(Description = "Test that both depth and sites validation rules are triggered when both parameters are invalid.")]
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

        [Test(Description = "Test that all validation rules pass when valid parameters are provided.")]
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
            yield return new TestCaseData(new List<string> { "site1", " " })
                .SetName("Sites containing whitespace");

            yield return new TestCaseData(new List<string> { "site1", "" })
                .SetName("Sites containing empty string");

            yield return new TestCaseData(new List<string> { " " })
                .SetName("Sites containing only whitespace");

            yield return new TestCaseData(new List<string> { "" })
                .SetName("Sites containing only empty string");
        }
    }
}
