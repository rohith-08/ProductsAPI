using AutoMapper;
using FluentAssertions;
using Moq;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Application.Mappings;
using ProductsAPI.Application.Services;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Domain.Exceptions;
using Xunit;

namespace ProductsAPI.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly IMapper _mapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepository = new Mock<IProductRepository>();

        _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepository.Object);

        var mapperConfig = new MapperConfiguration(
     cfg => cfg.AddProfile<MappingProfile>(),
     Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();

        _productService = new ProductService(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedByAndCreatedOn_WhenValidDtoProvided()
    {
        // Arrange
        var dto = new CreateProductDto { ProductName = "Test Product" };
        var createdBy = "testuser";

        _mockProductRepository
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        // Act
        var result = await _productService.CreateAsync(dto, createdBy);

        // Assert
        result.Should().NotBeNull();
        result.ProductName.Should().Be("Test Product");
        result.CreatedBy.Should().Be(createdBy);
        _mockProductRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductRepository
            .Setup(r => r.GetByIdWithItemsAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = async () => await _productService.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductDto_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            ProductName = "Existing Product",
            CreatedBy = "admin",
            CreatedOn = DateTime.UtcNow
        };

        _mockProductRepository
            .Setup(r => r.GetByIdWithItemsAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.ProductName.Should().Be("Existing Product");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        _mockProductRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        var dto = new UpdateProductDto { ProductName = "Updated Name" };

       
        Func<Task> act = async () => await _productService.UpdateAsync(1, dto, "admin");

        
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetModifiedByAndModifiedOn_WhenProductExists()
    {
        
        var existingProduct = new Product
        {
            Id = 1,
            ProductName = "Old Name",
            CreatedBy = "admin",
            CreatedOn = DateTime.UtcNow.AddDays(-1)
        };

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        var dto = new UpdateProductDto { ProductName = "New Name" };

        
        var result = await _productService.UpdateAsync(1, dto, "editor");

        
        result.ProductName.Should().Be("New Name");
        existingProduct.ModifiedBy.Should().Be("editor");
        existingProduct.ModifiedOn.Should().NotBeNull();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        _mockProductRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

       
        Func<Task> act = async () => await _productService.DeleteAsync(1);

        
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDeleteAndSave_WhenProductExists()
    {
        
        var product = new Product { Id = 1, ProductName = "To Delete" };

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

       
        await _productService.DeleteAsync(1);

    
        _mockProductRepository.Verify(r => r.DeleteAsync(product), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult_WithCorrectPagination()
    {
        
        var products = new List<Product>
        {
            new() { Id = 1, ProductName = "Product 1" },
            new() { Id = 2, ProductName = "Product 2" },
            new() { Id = 3, ProductName = "Product 3" }
        };

        _mockProductRepository
            .Setup(r => r.GetAllWithItemsAsync())
            .ReturnsAsync(products);

        
        var result = await _productService.GetAllAsync(pageNumber: 1, pageSize: 2);

       
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }
}