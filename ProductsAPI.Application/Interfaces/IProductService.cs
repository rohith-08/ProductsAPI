using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsAPI.Application.DTOs;

namespace ProductsAPI.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(CreateProductDto dto, string createdBy);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, string modifiedBy);
        Task DeleteAsync(int id);

    }
}
