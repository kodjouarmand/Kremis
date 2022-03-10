using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IIdentityDocumentTypeRepository : IBaseRepository<IdentityDocumentType, int>
    {
        public void Update(IdentityDocumentType city);        
    }
}
