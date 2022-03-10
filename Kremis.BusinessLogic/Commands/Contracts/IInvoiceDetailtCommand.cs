using Kremis.Domain.Assemblers;
using System;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public interface IInvoiceDetailCommand : IBaseCommand<InvoiceDetailDto, int>
    {

    }
}