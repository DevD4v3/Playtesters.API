using FluentAssertions;
using Playtesters.API.Entities;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.Testers;

public class RevokeAllKeysApiTests : TestBase
{
    [Test]
    public async Task Post_WhenTestersExist_ShouldRevokeAllKeys()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        string[] testers = ["Carlos", "Maria", "Juan"];
        foreach (var name in testers)
        {
            var createRequest = new CreateTesterRequest(Name: name);
            await client.PostAsJsonAsync("/api/testers", createRequest);
        }

        // Act
        var response = await client.PostAsJsonAsync("/api/testers/revoke-all-keys", new { });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<RevokeAllKeysResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.RevokedCount.Should().Be(testers.Length);

        var testersInDb = await ToListAsync<Tester>();
        testersInDb.Should().OnlyContain(t => t.AccessKey == null);
    }

    [Test]
    public async Task Post_WhenNoTestersExist_ShouldReturnZeroRevoked()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();

        // Act
        var response = await client.PostAsJsonAsync("/api/testers/revoke-all-keys", new { });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<RevokeAllKeysResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.RevokedCount.Should().Be(0);
    }

    [Test]
    public async Task Post_WhenMissingApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/testers/revoke-all-keys", new { });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
