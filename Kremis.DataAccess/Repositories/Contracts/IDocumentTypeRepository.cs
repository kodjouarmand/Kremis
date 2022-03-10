using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IDocumentTypeRepository : IBaseRepository<DocumentType, int>
    {
        public void Update(DocumentType documentType);        
    }
}
