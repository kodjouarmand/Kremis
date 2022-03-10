using Kremis.Domain.Assemblers;
using System.Collections.Generic;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface ICommissionPaymentQuery : IBaseQuery<CommissionPaymentDto, int>
    {
        IEnumerable<CommissionPaymentDto> GetByInvoiceHeaderId(int invoiceHeaderId);
    }
}