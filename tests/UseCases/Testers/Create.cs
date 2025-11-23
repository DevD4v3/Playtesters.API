using FluentAssertions;
using Playtesters.API.Entities;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.Testers;

public class CreateTesterApiTests : TestBase
{
    [Test]
    public async Task Post_WhenRequestIsValid_ShouldReturnCreatedTester()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var request = new CreateTesterRequest(Name: "Carlos");
        var expectedName = "Carlos";

        // Act
        var response = await client.PostAsJsonAsync("/api/testers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        var tester = await FirstOrDefaultAsync<Tester>(t => t.Name == expectedName);

        tester.Should().NotBeNull();
        tester.Name.Should().Be(body.Data.Name);
        tester.AccessKey.Should().Be(body.Data.AccessKey);
    }

    [Test]
    public async Task Post_WhenNameIsTooShort_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var request = new CreateTesterRequest(Name: "A");

        // Act
        var response = await client.PostAsJsonAsync("/api/testers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task Post_WhenTesterAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var request = new CreateTesterRequest(Name: "David");
        await client.PostAsJsonAsync("/api/testers", request);

        // Act
        var secondResponse = await client.PostAsJsonAsync("/api/testers", request);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var body = await secondResponse.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task Post_WhenMissingApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();
        var request = new CreateTesterRequest(Name: "María");

        // Act
        var response = await client.PostAsJsonAsync("/api/testers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
