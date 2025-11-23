using FluentValidation.TestHelper;
using Playtesters.API.UseCases.Testers;

namespace Playtesters.API.Tests.Validators;

public class ValidateTesterAccessValidatorTests
{
    [Test]
    public void AccessKey_WhenNull_ShouldHaveError()
    {
        // Arrange
        var validator = new ValidateTesterAccessValidator();
        var request = new ValidateTesterAccessRequest(AccessKey: null);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.AccessKey);
    }

    [Test]
    public void AccessKey_WhenEmpty_ShouldHaveError()
    {
        // Arrange
        var validator = new ValidateTesterAccessValidator();
        var request = new ValidateTesterAccessRequest(AccessKey: string.Empty);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.AccessKey);
    }

    [Test]
    public void AccessKey_WhenNotValidGuid_ShouldHaveError()
    {
        // Arrange
        var validator = new ValidateTesterAccessValidator();
        var request = new ValidateTesterAccessRequest(AccessKey: "not-a-guid");

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.AccessKey);
    }

    [Test]
    public void AccessKey_WhenValidGuid_ShouldNotHaveError()
    {
        // Arrange
        var validator = new ValidateTesterAccessValidator();
        var request = new ValidateTesterAccessRequest(AccessKey: Guid.NewGuid().ToString());

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.AccessKey);
    }
}
