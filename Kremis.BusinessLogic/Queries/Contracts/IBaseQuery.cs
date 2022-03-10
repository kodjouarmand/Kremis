using Kremis.Domain.Assemblers;
using System.Collections.Generic;
using Kremis.BusinessLogic.Enums;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface IBaseQuery<TDto, TEntityKey> where TDto : BaseDto<TEntityKey>
    {
        TDto GetById(TEntityKey id);
        IEnumerable<TDto> GetAll();        
    }
}