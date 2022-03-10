using Kremis.BusinessLogic.Queries;
using Kremis.Domain.Assemblers;
using System;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface ICustomerQuery : IBaseQuery<CustomerDto, int>
    {

    }
}