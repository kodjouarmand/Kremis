using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface ILocalityRepository : IBaseRepository<Locality, int>
    {
        public void Update(Locality locality);        
    }
}
