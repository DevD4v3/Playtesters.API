using FluentAssertions;
using Playtesters.API.Entities;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.Testers;

public class ValidateTesterAccessApiTests : TestBase
{
    [Test]
    public async Task Post_WhenAccessKeyIsValid_ShouldReturnTesterNameAndCreateHistory()
    {
        // Arrange
        var creatorClient = CreateHttpClientWithApiKey();
        var validatorClient = ApplicationFactory.CreateClient();
        var expectedName = "Carlos";
        var createRequest = new CreateTesterRequest(Name: expectedName);
        var createResponse = await creatorClient.PostAsJsonAsync("/api/testers", createRequest);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();

        var accessKey = createdBody.Data.AccessKey;
        var validateRequest = new ValidateTesterAccessRequest(AccessKey: accessKey);

        // Act
        var response = await validatorClient.PostAsJsonAsync("/api/testers/validate-access", validateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Result<ValidateTesterAccessResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Name.Should().Be(expectedName);

        var tester = await FirstOrDefaultAsync<Tester>(t => t.Name == expectedName);
        tester.Should().NotBeNull();

        var accessHistory = await FirstOrDefaultAsync<AccessValidationHistory>(h => h.TesterId == tester.Id);
        accessHistory.Should().NotBeNull();
        accessHistory.IpAddress.Should().Be("127.0.0.1");
        accessHistory.Country.Should().Be("Colombia");
        accessHistory.City.Should().Be("Medellin");
    }

    [Test]
    public async Task Post_WhenAccessKeyIsInvalidFormat_ShouldReturnBadRequest()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();
        var validateRequest = new ValidateTesterAccessRequest("NOT-A-GUID");

        // Act
        var response = await client.PostAsJsonAsync("/api/testers/validate-access", validateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task Post_WhenAccessKeyDoesNotExist_ShouldReturnInvalidCredentials()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();
        var validateRequest = new ValidateTesterAccessRequest(Guid.NewGuid().ToString());
        var createRequest = new CreateTesterRequest(Name: "Carlos");
        await client.PostAsJsonAsync("/api/testers", createRequest);

        // Act
        var response = await client.PostAsJsonAsync("/api/testers/validate-access", validateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<Result>();
        body.Should().NotBeNull();
        body.IsFailed.Should().BeTrue();
    }
}
