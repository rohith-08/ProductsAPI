using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ProductsAPI.Application.DTOs;
using Xunit;

namespace ProductsAPI.API.Tests;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var registerDto = new RegisterDto
        {
            Username = $"user_{Guid.NewGuid().ToString()[..8]}",
            Email = $"{Guid.NewGuid()}@test.com",
            Password = "Test@1234"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return result!.AccessToken;
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithoutAuthentication()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        // Arrange
        var dto = new CreateProductDto { ProductName = "Unauthorized Product" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenAuthenticated()
    {
        // Arrange
        var token = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateProductDto { ProductName = "Authenticated Product" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<ProductDto>();
        created!.ProductName.Should().Be("Authenticated Product");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var token = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateProductDto { ProductName = "Findable Product" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductDto>();
        result!.ProductName.Should().Be("Findable Product");
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenAuthenticatedAndProductExists()
    {
        // Arrange
        var token = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateProductDto { ProductName = "Original Name" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateDto = new UpdateProductDto { ProductName = "Updated Name" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{created!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductDto>();
        result!.ProductName.Should().Be("Updated Name");
    }
}