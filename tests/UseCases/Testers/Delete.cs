using FluentAssertions;
using Playtesters.API.Entities;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.Testers;

public class DeleteTesterApiTests : TestBase
{
    [Test]
    public async Task Delete_WhenTesterExists_ShouldDeleteTester()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();

        var createRequest = new CreateTesterRequest(Name: "Alice");
        await client.PostAsJsonAsync("/api/testers", createRequest);

        // Act
        var response = await client.DeleteAsync("/api/testers/Alice");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<DeleteTesterResponse>>();

        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Name.Should().Be("Alice");

        var tester = await FirstOrDefaultAsync<Tester>(t => t.Name == createRequest.Name);
        tester.Should().BeNull();
    }

    [Test]
    public async Task Delete_WhenTesterDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();

        // Act
        var response = await client.DeleteAsync("/api/testers/Unknown");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await response.Content.ReadFromJsonAsync<Result<DeleteTesterResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeFalse();
        body.Status.Should().Be(ResultStatus.NotFound);
    }

    [Test]
    public async Task Delete_WhenTesterHasAccessHistory_ShouldDeleteAccessHistory()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Alice");
        var createResponse = await client.PostAsJsonAsync("/api/testers", createRequest);
        var createdTester = await createResponse.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        var validateRequest = new ValidateTesterAccessRequest(createdTester.Data.AccessKey);
        await client.PostAsJsonAsync("/api/testers/validate-access", validateRequest);
        Count<AccessValidationHistory>().Should().Be(1);

        // Act
        var response = await client.DeleteAsync("/api/testers/Alice");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Count<Tester>().Should().Be(0);
        Count<AccessValidationHistory>().Should().Be(0);
    }

    [Test]
    public async Task Delete_WhenMissingApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/testers/Alice");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
