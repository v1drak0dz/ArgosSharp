using ArgosSharp.Api.DTOs.Job.CreateJob;
using ArgosSharp.Api.Validators;
using FluentAssertions;

namespace ArgosSharp.Api.UnitTests.Validators
{
    [TestFixture]
    public class CreateJobValidatorTests
    {
        private CreateJobValidator _validator;

        [SetUp]
        public void Setup()
            => _validator = new CreateJobValidator();

        [TestCase(null, TestName = "Test that the SearchTerm validation rule is triggered when the SearchTerm is null.")]
        [TestCase("", TestName = "Test that the SearchTerm validation rule is triggered when the SearchTerm is empty.")]
        [TestCase(" ", TestName = "Test that the SearchTerm validation rule is triggered when the SearchTerm is whitespace.")]
        [TestCase("  ", TestName = "Test that the SearchTerm validation rule is triggered when the SearchTerm is whitespace.")]
        public void SearchTermRule_WhenSearchTermIsNullEmptyOrWhitespace_ShouldHaveValidationError(
            string? searchTerm)
        {
            // Arrange
            var request = new CreateJobRequest
            {
                SearchTerm = searchTerm,
                Parameters = CreateValidParameters()
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();

            result.Errors.Should().ContainSingle(error =>
                error.PropertyName == nameof(CreateJobRequest.SearchTerm)
                && error.ErrorMessage == "SearchTerm is required.");
        }

        [Test(Description = "Test that the Parameters validation rule is triggered when the Parameters is null.")]
        public void ParametersRule_WhenParametersIsNull_ShouldHaveValidationError()
        {
            // Arrange
            var request = new CreateJobRequest
            {
                SearchTerm = "Test",
                Parameters = null
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();

            result.Errors.Should().ContainSingle(error =>
                error.PropertyName == nameof(CreateJobRequest.Parameters)
                && error.ErrorMessage == "Parameters are required.");
        }

        [Test(Description = "Test that both SearchTerm and Parameters validation rules are triggered when both are invalid.")]
        public void SearchTermAndParametersRules_WhenBothAreInvalid_ShouldHaveValidationErrors()
        {
            // Arrange
            var request = new CreateJobRequest
            {
                SearchTerm = "",
                Parameters = null
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();

            result.Errors.Should().HaveCount(2);

            result.Errors.Should().ContainSingle(error =>
                error.PropertyName == nameof(CreateJobRequest.SearchTerm)
                && error.ErrorMessage == "SearchTerm is required.");

            result.Errors.Should().ContainSingle(error =>
                error.PropertyName == nameof(CreateJobRequest.Parameters)
                && error.ErrorMessage == "Parameters are required.");
        }

        [Test(Description = "Test that all validation rules pass when valid parameters are provided.")]
        public void CreateJobRules_WhenRequestIsValid_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var request = new CreateJobRequest
            {
                SearchTerm = "Test",
                Parameters = CreateValidParameters()
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        private static CreateJobParametersRequest CreateValidParameters()
        {
            return new CreateJobParametersRequest
            {
                Depth = 1,
                Sites = ["site1"]
            };
        }
    }
}