using Kremis.Domain.Entities;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class ParcelRepository : BaseRepository<Parcel, int>, IParcelRepository
    {
        public ParcelRepository(ApplicationDbContext dbContext) : base(dbContext) 
        { 
        }

        public virtual void Update(Parcel parcelToUpdate)
        {
            var originalEntity = GetById(parcelToUpdate.Id);

            if (!string.IsNullOrWhiteSpace(parcelToUpdate.Number)) originalEntity.Number = parcelToUpdate.Number;
            originalEntity.LandTitleId = parcelToUpdate.LandTitleId;
            originalEntity.LocalityId = parcelToUpdate.LocalityId;
            originalEntity.Surface = parcelToUpdate.Surface;
            originalEntity.UnitPrice = parcelToUpdate.UnitPrice;
            originalEntity.MinimumUnitPrice = parcelToUpdate.MinimumUnitPrice;
            originalEntity.BlocNumber = parcelToUpdate.BlocNumber;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.LandType)) originalEntity.LandType = parcelToUpdate.LandType;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.RoadType)) originalEntity.RoadType = parcelToUpdate.RoadType;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.AreaMarking)) originalEntity.AreaMarking = parcelToUpdate.AreaMarking;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.HasElectrilocality)) originalEntity.HasElectrilocality = parcelToUpdate.HasElectrilocality;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.HasWater)) originalEntity.HasWater = parcelToUpdate.HasWater;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.HasTechnicalFile)) originalEntity.HasTechnicalFile = parcelToUpdate.HasTechnicalFile;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.HasImages)) originalEntity.HasImages = parcelToUpdate.HasImages;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.HasVideos)) originalEntity.HasVideos = parcelToUpdate.HasVideos;
            if (!string.IsNullOrWhiteSpace(parcelToUpdate.Status)) originalEntity.Status = parcelToUpdate.Status;
            originalEntity.Description = parcelToUpdate.Description;
            originalEntity.LastModificationDate = parcelToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = parcelToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }

        public virtual void UpdateStatus(Parcel parcelToUpdate)
        {
            var originalEntity = GetById(parcelToUpdate.Id);

            if (!string.IsNullOrWhiteSpace(parcelToUpdate.Status)) originalEntity.Status = parcelToUpdate.Status;
            originalEntity.LastModificationDate = parcelToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = parcelToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
