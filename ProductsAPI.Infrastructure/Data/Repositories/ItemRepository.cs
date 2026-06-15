using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Infrastructure.Data.Repositories
{
    public class ItemRepository: GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Item>> GetByProductIdAsync(int productId)
        {
            return await _context.Items
                .Where(i => i.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
    
}
