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
    public class ProductRepository: GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Product>> GetAllWithItemsAsync()
        {
            return await _context.Products
                    .Include(p=>p.Items)
                    .AsNoTracking()
                    .ToListAsync();
        }
        public async Task<Product?> GetByIdWithItemsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }


    }
}
