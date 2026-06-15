using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsAPI.Application.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        IProductRepository Products { get; }
        IItemRepository Items { get; }
        Task<int> SaveChangesAsync();

    }
}
