using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface ICustomerDocumentRepository : IBaseRepository<CustomerDocument, int>
    {
        public void Update(CustomerDocument customerDocument);        
    }
}
