using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface ILandTitleDocumentQuery : IBaseQuery<LandTitleDocumentDto, int>
    {
        LandTitleDocumentDto GetByDocumentUrl(string documentUrld);
        IEnumerable<LandTitleDocumentDto> GetByLandTitleId(int landTitleId);
    }
}