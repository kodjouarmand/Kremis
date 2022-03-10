using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface ILandTitleQuery : IBaseQuery<LandTitleDto, int>
    {
        IEnumerable<LandTitleDto> GetByLocalityId(int cityId);
        LandTitleDto GetByNumber(string number);
    }
}