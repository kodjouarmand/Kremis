using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IInvoiceHeaderRepository : IBaseRepository<InvoiceHeader, int>
    {
        public void Update(InvoiceHeader invoiceHeader);
    }
}
