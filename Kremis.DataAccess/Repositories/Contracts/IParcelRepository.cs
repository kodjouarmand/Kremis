using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IParcelRepository : IBaseRepository<Parcel, int>
    {
        public void Update(Parcel parcel);
        void UpdateStatus(Parcel parcelToUpdate);
    }
}
