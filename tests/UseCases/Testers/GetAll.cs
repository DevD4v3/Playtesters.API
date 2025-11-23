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
