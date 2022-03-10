using Kremis.BusinessLogic.Queries;
using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface IParcelQuery : IBaseQuery<ParcelDto, int>
    {
        IEnumerable<ParcelDto> GetByLandTitleId(int landTitleId);
        IEnumerable<ParcelDto> GetByStatus(string status);
    }
}