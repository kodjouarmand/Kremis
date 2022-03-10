using Kremis.Domain.Assemblers;
using System;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public interface ILocalityCommand : IBaseCommand<LocalityDto, int>
    {

    }
}