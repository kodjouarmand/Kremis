using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface ILandTitleDocumentRepository : IBaseRepository<LandTitleDocument, int>
    {
        public void Update(LandTitleDocument landTitleDocument);        
    }
}
