using FluentAssertions;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.Middlewares;

public class ApiKeyMiddlewareTests : TestBase
{
    [Test]
    public async Task Post_WhenCreatingTesterWithoutApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();
        var request = new CreateTesterRequest(Name: "Tester1");
        var expectedMessage = "Missing API Key.";

        // Act
        var response = await client.PostAsJsonAsync("/api/testers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var body = await response.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.Message.Should().Be(expectedMessage);
    }

    [Test]
    public async Task Post_WhenCreatingTesterWithInvalidApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();
        var request = new CreateTesterRequest(Name: "Tester1");
        var expectedMessage = "Invalid API Key.";
        client.DefaultRequestHeaders.Add("X-Api-Key", "INVALID_KEY");

        // Act
        var response = await client.PostAsJsonAsync("/api/testers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var body = await response.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.Message.Should().Be(expectedMessage);
    }

    [Test]
    public async Task Post_WhenCreatingTesterWithValidApiKey_ShouldReturnOk()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();
        var request = new CreateTesterRequest(Name: "TesterValid");
        var apiKey = Environment.GetEnvironmentVariable("API_KEY");
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

        // Act
        var response = await client.PostAsJsonAsync("/api/testers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        body.Should().NotBeNull();
        body.Data.Name.Should().Be(request.Name);
        body.Data.AccessKey.Should().NotBeNullOrEmpty();
    }
}
