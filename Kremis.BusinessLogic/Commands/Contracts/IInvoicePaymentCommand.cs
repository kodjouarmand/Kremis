using Kremis.Domain.Assemblers;
using System;
using System.Text;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public interface IInvoicePaymentCommand : IBaseCommand<InvoicePaymentDto, int>
    {

    }
}