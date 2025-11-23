using FluentValidation.TestHelper;
using Playtesters.API.UseCases.Testers;

namespace Playtesters.API.Tests.Validators;

public class CreateTesterValidatorTests
{
    [Test]
    public void Name_WhenNull_ShouldHaveError()
    {
        // Arrange
        var validator = new CreateTesterValidator();
        var request = new CreateTesterRequest(Name: null);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Test]
    public void Name_WhenEmpty_ShouldHaveError()
    {
        // Arrange
        var validator = new CreateTesterValidator();
        var request = new CreateTesterRequest(Name: string.Empty);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Test]
    public void Name_WhenShorterThanThreeCharacters_ShouldHaveError()
    {
        // Arrange
        var validator = new CreateTesterValidator();
        var request = new CreateTesterRequest(Name: "ab");

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Test]
    public void Name_WhenHasThreeCharacters_ShouldNotHaveError()
    {
        // Arrange
        var validator = new CreateTesterValidator();
        var request = new CreateTesterRequest(Name: "abc");

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Name);
    }

    [Test]
    public void Name_WhenLongerThanThreeCharacters_ShouldNotHaveError()
    {
        // Arrange
        var validator = new CreateTesterValidator();
        var request = new CreateTesterRequest(Name: "tester");

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Name);
    }
}
