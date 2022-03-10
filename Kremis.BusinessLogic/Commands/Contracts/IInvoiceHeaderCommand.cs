using Kremis.BusinessLogic.Queries;
using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Text;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public interface IInvoiceHeaderCommand : IBaseCommand<InvoiceHeaderDto, int>
    {
        void UpdateStatus(ref InvoiceHeader invoiceHeader);
        void UpdateAmounts(ref InvoiceHeader invoiceHeader);
        void UpdateCommissionStatus(ref InvoiceHeader invoiceHeader);
    }
}