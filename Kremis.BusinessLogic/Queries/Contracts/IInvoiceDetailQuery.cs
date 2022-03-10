using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface IInvoiceDetailQuery : IBaseQuery<InvoiceDetailDto, int>
    {
        IEnumerable<InvoiceDetailDto> GetByInvoiceHeaderId(int invoiceHeaderId);
        InvoiceDetailDto GetByParcelId(int parcelId);
    }
}