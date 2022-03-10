using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IParcelDocumentRepository : IBaseRepository<ParcelDocument, int>
    {
        public void Update(ParcelDocument parcelDocument);        
    }
}
