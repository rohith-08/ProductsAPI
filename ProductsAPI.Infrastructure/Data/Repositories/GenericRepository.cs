using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProductsAPI.Application.Interfaces;


namespace ProductsAPI.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> :IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
         
                _context = context;
                _dbSet = _context.Set<T>();
         }
            public async Task<IEnumerable<T>> GetAllAsync()
            {
            return await _dbSet.AsNoTracking().ToListAsync();

             }
           public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }


    }
}
