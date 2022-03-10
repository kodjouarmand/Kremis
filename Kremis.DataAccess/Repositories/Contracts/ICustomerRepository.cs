using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface ICustomerRepository : IBaseRepository<Customer, int>
    {
        public void Update(Customer customer);        
    }
}
