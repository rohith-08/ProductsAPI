using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProductsAPI.Application.DTOs;
using Xunit;

namespace ProductsAPI.API.Tests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnOkWithAccessToken_WhenValidDataProvided()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = $"user_{Guid.NewGuid().ToString()[..8]}",
            Email = $"{Guid.NewGuid()}@test.com",
            Password = "Test@1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto
        {
            Username = $"user_{Guid.NewGuid().ToString()[..8]}",
            Email = email,
            Password = "Test@1234"
        };

        await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        var duplicateDto = new RegisterDto
        {
            Username = $"user_{Guid.NewGuid().ToString()[..8]}",
            Email = email, // same email
            Password = "Test@1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", duplicateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithToken_WhenCredentialsAreValid()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@test.com";
        var password = "Test@1234";

        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterDto
        {
            Username = $"user_{Guid.NewGuid().ToString()[..8]}",
            Email = email,
            Password = password
        });

        var loginDto = new LoginDto { Email = email, Password = password };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsWrong()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@test.com";

        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterDto
        {
            Username = $"user_{Guid.NewGuid().ToString()[..8]}",
            Email = email,
            Password = "CorrectPassword1"
        });

        var loginDto = new LoginDto { Email = email, Password = "WrongPassword" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
