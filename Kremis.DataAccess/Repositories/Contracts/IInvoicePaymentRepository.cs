using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IInvoicePaymentRepository : IBaseRepository<InvoicePayment, int>
    {
        public void Update(InvoicePayment invoicePayment);
    }
}
