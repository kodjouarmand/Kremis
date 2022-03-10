using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface IInvoicePaymentQuery : IBaseQuery<InvoicePaymentDto, int>
    {
        IEnumerable<InvoicePaymentDto> GetByInvoiceHeaderId(int invoiceHeaderId);
    }
}