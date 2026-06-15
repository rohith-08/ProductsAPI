using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Application.Interfaces
{
    public interface IItemRepository:IRepository<Item>
    {
        Task<IEnumerable<Item>> GetByProductIdAsync(int productId);
    }
}
