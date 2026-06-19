using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Data;
using ProductsAPI.Infrastructure.Data.Repositories;
using Xunit;

namespace ProductsAPI.Infrastructure.Tests;

public class ProductRepositoryTests
{
    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ProductRepository(context);

        var product = new Product
        {
            ProductName = "Test Product",
            CreatedBy = "tester",
            CreatedOn = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(product);
        await context.SaveChangesAsync();

        // Assert
        var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Test Product");
        savedProduct.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdWithItemsAsync_ShouldReturnProductWithItems()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ProductRepository(context);

        var product = new Product
        {
            ProductName = "Product With Items",
            CreatedBy = "tester",
            CreatedOn = DateTime.UtcNow,
            Items = new List<Item>
            {
                new() { Quantity = 5 },
                new() { Quantity = 10 }
            }
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdWithItemsAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllWithItemsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ProductRepository(context);

        context.Products.AddRange(
            new Product { ProductName = "Product A", CreatedBy = "tester", CreatedOn = DateTime.UtcNow },
            new Product { ProductName = "Product B", CreatedBy = "tester", CreatedOn = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllWithItemsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProductFromDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ProductRepository(context);

        var product = new Product
        {
            ProductName = "To Be Deleted",
            CreatedBy = "tester",
            CreatedOn = DateTime.UtcNow
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(product);
        await context.SaveChangesAsync();

        // Assert
        var deletedProduct = await context.Products.FindAsync(product.Id);
        deletedProduct.Should().BeNull();
    }
}
