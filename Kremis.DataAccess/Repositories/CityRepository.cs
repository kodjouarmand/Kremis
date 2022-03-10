using Kremis.Domain.Entities;
using System;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class CityRepository : BaseRepository<City, int>, ICityRepository
    {
        public CityRepository(ApplicationDbContext dbContext) : base(dbContext) 
        { 
        }

        public virtual void Update(City cityToUpdate)
        {
            var originalEntity = GetById(cityToUpdate.Id);

            if (!string.IsNullOrWhiteSpace(cityToUpdate.Name)) originalEntity.Name = cityToUpdate.Name;
            originalEntity.LastModificationDate = cityToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = cityToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
