using FluentAssertions;
using Playtesters.API.Entities;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.Testers;

public class UpdateTesterApiTests : TestBase
{
    [Test]
    public async Task Patch_WhenTesterExists_ShouldUpdateAndReturnUpdatedTester()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Carlos");
        await client.PostAsJsonAsync("/api/testers", createRequest);

        var newAccessKey = Guid.NewGuid().ToString();
        var updateRequest = new UpdateTesterRequest(AccessKey: newAccessKey);
        var expectedName = "Carlos";

        // Act
        var response = await client.PatchAsJsonAsync("/api/testers/Carlos", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<UpdateTesterResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Name.Should().Be(expectedName);
        body.Data.AccessKey.Should().Be(newAccessKey);
    }

    [Test]
    public async Task Patch_WhenTesterUpdated_ShouldPersistChangesInDatabase()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Maria");
        await client.PostAsJsonAsync("/api/testers", createRequest);

        var updatedKey = Guid.NewGuid().ToString();
        var updateRequest = new UpdateTesterRequest(updatedKey);
        var expectedName = "Maria";

        // Act
        var response = await client.PatchAsJsonAsync("/api/testers/Maria", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tester = await FirstOrDefaultAsync<Tester>(t => t.Name == expectedName);
        tester.Should().NotBeNull();
        tester.AccessKey.Should().Be(updatedKey);
    }

    [Test]
    public async Task Patch_WhenTesterDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Juan");
        await client.PostAsJsonAsync("/api/testers", createRequest);

        var updateRequest = new UpdateTesterRequest(AccessKey: Guid.NewGuid().ToString());

        // Act
        var response = await client.PatchAsJsonAsync("/api/testers/NotExist", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await response.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task Patch_WhenAccessKeyIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var updateRequest = new UpdateTesterRequest("NOT-A-GUID");

        // Act
        var response = await client.PatchAsJsonAsync("/api/testers/Juan", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task Patch_WhenAccessKeyIsEmpty_ShouldRevokeKey()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Lucia");
        await client.PostAsJsonAsync("/api/testers", createRequest);

        var initialKey = Guid.NewGuid().ToString();
        var assignRequest = new UpdateTesterRequest(AccessKey: initialKey);
        await client.PatchAsJsonAsync("/api/testers/Lucia", assignRequest);

        var revokeRequest = new UpdateTesterRequest(AccessKey: string.Empty);
        var expectedName = "Lucia";

        // Act
        var response = await client.PatchAsJsonAsync("/api/testers/Lucia", revokeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<UpdateTesterResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Name.Should().Be(expectedName);
        body.Data.AccessKey.Should().BeNull();

        var tester = await FirstOrDefaultAsync<Tester>(t => t.Name == expectedName);
        tester.Should().NotBeNull();
        tester.AccessKey.Should().BeNull();
    }

    [Test]
    public async Task Patch_WhenAccessKeyIsNull_ShouldNotModifyExistingKey()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Ana");
        await client.PostAsJsonAsync("/api/testers", createRequest);

        var initialKey = Guid.NewGuid().ToString();
        var assignRequest = new UpdateTesterRequest(AccessKey: initialKey);
        await client.PatchAsJsonAsync("/api/testers/Ana", assignRequest);

        var noChangeRequest = new UpdateTesterRequest(AccessKey: null);
        var expectedName = "Ana";

        // Act
        var response = await client.PatchAsJsonAsync("/api/testers/Ana", noChangeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<UpdateTesterResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Name.Should().Be(expectedName);
        body.Data.AccessKey.Should().Be(initialKey);

        var tester = await FirstOrDefaultAsync<Tester>(t => t.Name == expectedName);
        tester.Should().NotBeNull();
        tester.AccessKey.Should().Be(initialKey);
    }

    [Test]
    public async Task Patch_WhenMissingApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();
        var updateRequest = new UpdateTesterRequest(AccessKey: Guid.NewGuid().ToString());

        // Act
        var response = await client.PatchAsJsonAsync("/api/testers/Pepe", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
