using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface ICityRepository : IBaseRepository<City, int>
    {
        public void Update(City city);        
    }
}
