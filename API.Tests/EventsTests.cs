using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace API.Tests;

public class EventsTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EventsTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Event_Should_Succeed()
    {
        var request = new
        {
            deviceId = "test-device",
            timestamp = DateTime.UtcNow,
            temperature = 30.5m,
            status = "OK",
            metadata = new
            {
                modelVersion = "v1",
                confidence = 0.95
            }
        };

        var response = await _client
            .PostAsJsonAsync(
                "/api/events",
                request);

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();
    }
}