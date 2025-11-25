using FluentAssertions;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.Testers;

public class GetTestersApiTests : TestBase
{
    [Test]
    public async Task Get_WhenTestersExist_ShouldReturnAllTesters()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var testers = new[] { "Carlos", "Maria", "Juan" };
        foreach (var name in testers)
        {
            var createRequest = new CreateTesterRequest(Name: name);
            await client.PostAsJsonAsync("/api/testers", createRequest);
        }

        // Act
        var response = await client.GetAsync("/api/testers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListedResult<GetTestersResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Count().Should().Be(testers.Length);

        var returnedNames = body.Data.Select(t => t.Name).ToList();
        returnedNames.Should().BeEquivalentTo(testers);
    }

    [Test]
    public async Task Get_WhenTesterHasPlaytime_ShouldReturnFormattedPlaytime()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Carlos");
        var createResponse = await client.PostAsJsonAsync("/api/testers", createRequest);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        var accessKey = createdBody.Data.AccessKey;

        // Update playtime (for example 2.75 hours = 02:45:00)
        var playtimeRequest = new UpdatePlaytimeRequest(HoursPlayed: 2.75);
        var requestUri = $"/api/testers/{accessKey}/playtime";
        var playtimeResponse = await client.PatchAsJsonAsync(requestUri, playtimeRequest);
        var expectedPlaytime = "02:45:00";

        // Act
        var response = await client.GetAsync("/api/testers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListedResult<GetTestersResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();

        var tester = body.Data.FirstOrDefault(t => t.Name == "Carlos");
        tester.TotalPlaytime.Should().Be(expectedPlaytime);
    }

    [Test]
    public async Task Get_WhenNoTestersExist_ShouldReturnEmptyList()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();

        // Act
        var response = await client.GetAsync("/api/testers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ListedResult<GetTestersResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Should().BeEmpty();
    }

    [Test]
    public async Task Get_WhenMissingApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/testers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
