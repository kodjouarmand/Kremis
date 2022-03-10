using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface ICustomerDocumentQuery : IBaseQuery<CustomerDocumentDto, int>
    {
        CustomerDocumentDto GetByDocumentUrl(string documentUrld);
        IEnumerable<CustomerDocumentDto> GetByCustomerId(int customerId);
    }
}