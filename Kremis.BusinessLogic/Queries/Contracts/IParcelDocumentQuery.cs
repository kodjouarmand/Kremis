using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Queries.Contracts
{
    public interface IParcelDocumentQuery : IBaseQuery<ParcelDocumentDto, int>
    {
        ParcelDocumentDto GetByDocumentUrl(string documentUrld);
        IEnumerable<ParcelDocumentDto> GetByParcelId(int parcelId);
    }
}