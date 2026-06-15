using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsAPI.Domain.Exceptions
{
    public class DomainException:Exception
    {
        public DomainException(string message) : base(message)
        {

        }
            

    }
    public class NotFoundException:DomainException
    {
        public NotFoundException(string entity, int id) : base($"{entity} with ID {id} was not found.")
        {
        }
    }
}
