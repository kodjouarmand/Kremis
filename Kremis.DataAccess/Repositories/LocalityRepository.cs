using Kremis.Domain.Entities;
using System;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class LocalityRepository : BaseRepository<Locality, int>, ILocalityRepository
    {
        public LocalityRepository(ApplicationDbContext dbContext) : base(dbContext) 
        { 
        }

        public virtual void Update(Locality localityToUpdate)
        {
            var originalEntity = GetById(localityToUpdate.Id);

            originalEntity.CityId = localityToUpdate.CityId;
            if (!string.IsNullOrWhiteSpace(localityToUpdate.Name)) originalEntity.Name = localityToUpdate.Name;      
            if (!string.IsNullOrWhiteSpace(localityToUpdate.Description)) originalEntity.Description = localityToUpdate.Description;
            originalEntity.LastModificationDate = localityToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = localityToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
