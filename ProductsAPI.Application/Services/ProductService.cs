using AutoMapper;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Domain.Exceptions;

namespace ProductsAPI.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ProductDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var allProducts = await _unitOfWork.Products.GetAllWithItemsAsync();
        var totalCount = allProducts.Count();

        var paged = allProducts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResultDto<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(paged),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdWithItemsAsync(id);

        if (product == null)
            throw new NotFoundException("Product", id);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, string createdBy)
    {
        var product = _mapper.Map<Product>(dto);
        product.CreatedBy = createdBy;
        product.CreatedOn = DateTime.UtcNow;

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, string modifiedBy)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
            throw new NotFoundException("Product", id);

        product.ProductName = dto.ProductName;
        product.ModifiedBy = modifiedBy;
        product.ModifiedOn = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
            throw new NotFoundException("Product", id);

        await _unitOfWork.Products.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }
}