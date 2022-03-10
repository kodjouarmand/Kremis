using Kremis.BusinessLogic.Queries;
using Kremis.Domain.Assemblers;
using System;
using System.Collections.Generic;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface IInvoiceHeaderQuery : IBaseQuery<InvoiceHeaderDto, int>
    {
        InvoiceHeaderDto GetByNumber(string number);
        IEnumerable<InvoiceHeaderDto> GetCommissions();
        IEnumerable<InvoiceHeaderDto> GetUnpaidCommissions();
    }
}