using Kremis.BusinessLogic.Queries;
using Kremis.Domain.Assemblers;
using System;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public interface IBusinessPartnerCommand : IBaseCommand<BusinessPartnerDto, int>
    {

    }
}