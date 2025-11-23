using FluentValidation.TestHelper;
using Playtesters.API.UseCases.TesterAccessHistory;

namespace Playtesters.API.Tests.Validators;

public class GetAllTestersAccessHistoryValidatorTests
{
    [Test]
    public void PageNumber_WhenLessOrEqualToZero_ShouldHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: null,
            ToDate: null,
            PageNumber: 0,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.PageNumber);
    }

    [Test]
    public void PageNumber_WhenGreaterThanZero_ShouldNotHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: null,
            ToDate: null,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.PageNumber);
    }

    [Test]
    public void PageSize_WhenLessThan10_ShouldHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: null,
            ToDate: null,
            PageNumber: 1,
            PageSize: 5
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.PageSize);
    }

    [Test]
    public void PageSize_WhenGreaterThan100_ShouldHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: null,
            ToDate: null,
            PageNumber: 1,
            PageSize: 150
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.PageSize);
    }

    [Test]
    public void PageSize_WhenWithinRange_ShouldNotHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: null,
            ToDate: null,
            PageNumber: 1,
            PageSize: 50
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.PageSize);
    }

    [Test]
    public void FromDate_WhenInvalidFormat_ShouldHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: "2024/05/01",
            ToDate: null,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.FromDate);
    }

    [Test]
    public void FromDate_WhenValidFormat_ShouldNotHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: "2024-05-01",
            ToDate: null,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.FromDate);
    }

    [Test]
    public void ToDate_WhenInvalidFormat_ShouldHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: null,
            ToDate: "05-01-2024",
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.ToDate);
    }

    [Test]
    public void ToDate_WhenValidFormat_ShouldNotHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: null,
            ToDate: "2024-05-01",
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.ToDate);
    }

    [Test]
    public void DateRange_WhenFromDateAfterToDate_ShouldHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: "2024-05-10",
            ToDate: "2024-05-01",
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r);
    }

    [Test]
    public void DateRange_WhenFromDateBeforeOrEqualToDate_ShouldNotHaveError()
    {
        // Arrange
        var validator = new GetAllTestersAccessHistoryValidator();
        var request = new GetAllTestersAccessHistoryRequest(
            Name: null,
            IpAddress: null,
            Country: null,
            FromDate: "2024-05-01",
            ToDate: "2024-05-10",
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r);
    }
}
