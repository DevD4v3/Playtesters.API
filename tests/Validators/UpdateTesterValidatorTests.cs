using FluentValidation.TestHelper;
using Playtesters.API.UseCases.Testers;

namespace Playtesters.API.Tests.Validators;

public class UpdateTesterValidatorTests
{
    [Test]
    public void AccessKey_WhenNull_ShouldNotHaveError()
    {
        // Arrange
        var validator = new UpdateTesterValidator();
        var request = new UpdateTesterRequest(AccessKey: null);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.AccessKey);
    }

    [Test]
    public void AccessKey_WhenEmpty_ShouldNotHaveError()
    {
        // Arrange
        var validator = new UpdateTesterValidator();
        var request = new UpdateTesterRequest(AccessKey: string.Empty);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.AccessKey);
    }

    [Test]
    public void AccessKey_WhenNotValidGuid_ShouldHaveError()
    {
        // Arrange
        var validator = new UpdateTesterValidator();
        var request = new UpdateTesterRequest(AccessKey: "not-a-guid");

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.AccessKey);
    }

    [Test]
    public void AccessKey_WhenValidGuid_ShouldNotHaveError()
    {
        // Arrange
        var validator = new UpdateTesterValidator();
        var request = new UpdateTesterRequest(AccessKey: Guid.NewGuid().ToString());

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.AccessKey);
    }
}
