using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IInvoiceDetailRepository : IBaseRepository<InvoiceDetail, int>
    {
        public void Update(InvoiceDetail invoiceDetail);        
    }
}
