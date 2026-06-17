using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsAPI.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }=string.Empty;
        public string CreatedBy { get; set; }=string.Empty;

        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public List<ItemDto> Items { get; set; } = new();

    }
    public class CreateProductDto
    {
        public string ProductName { get; set; } = string.Empty;

    }

    public class UpdateProductDto
    {
        public string ProductName { get; set; } = string.Empty;

    }
}
