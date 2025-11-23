using FluentAssertions;
using Playtesters.API.Entities;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.TesterAccessHistory;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.TesterAccessHistory;

public class GetAllTestersAccessHistoryApiTests : TestBase
{
    private async Task<int> CreateTesterAsync(
        string name, 
        string ip = "127.0.0.1", 
        string country = "Colombia", 
        string city = "Medellin")
    {
        var client = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(name);
        var createResponse = await client.PostAsJsonAsync("/api/testers", createRequest);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        var accessKey = createdBody.Data.AccessKey;

        var tester = await FirstOrDefaultAsync<Tester>(t => t.Name == name);
        List<AccessValidationHistory> accessValidationHistories =
        [
            new AccessValidationHistory
            {
                TesterId = tester.Id,
                CheckedAt = DateTime.Parse("2025-11-22 07:01:01"),
                IpAddress = ip,
                Country = country,
                City = city
            },
            new AccessValidationHistory
            {
                TesterId = tester.Id,
                CheckedAt = DateTime.Parse("2025-11-23 09:55:01"),
                IpAddress = ip,
                Country = country,
                City = city
            },
            new AccessValidationHistory
            {
                TesterId = tester.Id,
                CheckedAt = DateTime.Parse("2025-11-23 12:33:01"),
                IpAddress = ip,
                Country = country,
                City = city
            }
        ];
        await AddRangeAsync(accessValidationHistories);
        return tester.Id;
    }

    [Test]
    public async Task Get_WhenNoFilters_ShouldReturnAllAccessHistory()
    {
        // Arrange
        int expectedRecords = 6;
        string[] expectedNames = ["Carlos", "Maria"];
        var client = CreateHttpClientWithApiKey();
        await CreateTesterAsync("Carlos");
        await CreateTesterAsync("Maria");

        var request = new GetAllTestersAccessHistoryRequest(
            Name: string.Empty,
            IpAddress: string.Empty,
            Country: string.Empty,
            FromDate: string.Empty,
            ToDate: string.Empty
        );
        var requestUri = $"/api/testers/access-history";

        // Act
        var response = await client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<GetAllTestersAccessHistoryResponse>>();
        body.Should().NotBeNull();
        body.IsSuccess.Should().BeTrue();
        body.Data.Count().Should().Be(expectedRecords);

        var names = body.Data.Select(h => h.Name).ToList();
        names.Should().Contain(expectedNames);

        foreach (var accessHistory in body.Data)
        {
            accessHistory.IpAddress.Should().NotBeNullOrEmpty();
            accessHistory.Country.Should().NotBeNullOrEmpty();
            accessHistory.City.Should().NotBeNullOrEmpty();;
        }
    }

    [Test]
    public async Task Get_WhenFilteredByName_ShouldReturnOnlyMatchingTester()
    {
        // Arrange
        var expectedName = "Carlos";
        var client = CreateHttpClientWithApiKey();
        await CreateTesterAsync("Carlos");
        await CreateTesterAsync("Maria");

        var request = new GetAllTestersAccessHistoryRequest(
            Name: "Carlos",
            IpAddress: string.Empty,
            Country: string.Empty,
            FromDate: string.Empty,
            ToDate: string.Empty
        );
        var requestUri = $"/api/testers/access-history?" +
            $"Name={request.Name}";

        // Act
        var response = await client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<GetAllTestersAccessHistoryResponse>>();
        body.Should().NotBeNull();
        body.Data.Should().OnlyContain(h => h.Name == expectedName);
    }

    [Test]
    public async Task Get_WhenFilteredByIpAddress_ShouldReturnOnlyMatchingRecords()
    {
        // Arrange
        var expectedIpAddress = "1.2.3.4";
        var client = CreateHttpClientWithApiKey();
        await CreateTesterAsync("Carlos", ip: "1.2.3.4");
        await CreateTesterAsync("Maria", ip: "5.6.7.8");

        var request = new GetAllTestersAccessHistoryRequest(
            Name: string.Empty,
            IpAddress: "1.2.3.4",
            Country: string.Empty,
            FromDate: string.Empty,
            ToDate: string.Empty
        );
        var requestUri = $"/api/testers/access-history?" +
            $"IpAddress={request.IpAddress}";

        // Act
        var response = await client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<GetAllTestersAccessHistoryResponse>>();
        body.Should().NotBeNull();
        body.Data.Should().OnlyContain(h => h.IpAddress == expectedIpAddress);
    }

    [Test]
    public async Task Get_WhenFilteredByCountry_ShouldReturnOnlyMatchingRecords()
    {
        // Arrange
        var expectedCountry = "Colombia";
        var client = CreateHttpClientWithApiKey();
        await CreateTesterAsync("Carlos", country: "Colombia");
        await CreateTesterAsync("Maria", country: "Argentina");

        var request = new GetAllTestersAccessHistoryRequest(
            Name: string.Empty,
            IpAddress: string.Empty,
            Country: "Colombia",
            FromDate: string.Empty,
            ToDate: string.Empty
        );
        var requestUri = $"/api/testers/access-history?" +
            $"Country={request.Country}";

        // Act
        var response = await client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<GetAllTestersAccessHistoryResponse>>();
        body.Should().NotBeNull();
        body.Data.Should().OnlyContain(h => h.Country == expectedCountry);
    }

    [Test]
    public async Task Get_WhenFilteredByDateRange_ShouldReturnOnlyRecordsWithinRange()
    {
        // Arrange
        int expectedRecords = 4;
        string[] expectedNames = ["Carlos", "Maria"];
        var client = CreateHttpClientWithApiKey();
        await CreateTesterAsync("Carlos");
        await CreateTesterAsync("Maria");

        var request = new GetAllTestersAccessHistoryRequest(
            Name: string.Empty,
            IpAddress: string.Empty,
            Country: string.Empty,
            FromDate: "2025-11-23",
            ToDate: "2025-11-23"
        );
        var requestUri = $"/api/testers/access-history?" +
            $"FromDate={request.FromDate}&" + 
            $"ToDate={request.ToDate}";

        // Act
        var response = await client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<GetAllTestersAccessHistoryResponse>>();
        body.Should().NotBeNull();
        body.Data.Count().Should().Be(expectedRecords);

        var names = body.Data.Select(h => h.Name).ToList();
        names.Should().Contain(expectedNames);
    }

    [Test]
    public async Task Get_WhenNoRecordsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var client = CreateHttpClientWithApiKey();

        // Act
        var response = await client.GetAsync($"/api/testers/access-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<GetAllTestersAccessHistoryResponse>>();
        body.Should().NotBeNull();
        body.Data.Should().BeEmpty();
    }


    [Test]
    public async Task Get_WhenMissingApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = ApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/testers/access-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
