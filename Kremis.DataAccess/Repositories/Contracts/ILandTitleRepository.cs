using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface ILandTitleRepository : IBaseRepository<LandTitle, int>
    {
        public void Update(LandTitle landTitle);        
    }
}
