using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface ILocalityQuery : IBaseQuery<LocalityDto, int>
    {
        IEnumerable<LocalityDto> GetByCityId(int cityId);
        LocalityDto GetByNumber(string number);
    }
}