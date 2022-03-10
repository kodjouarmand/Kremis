using AutoMapper;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;

namespace Kremis.BusinessLogic.Queries
{
    public class ParcelQuery : BaseQuery<ParcelDto, Parcel, int>, IParcelQuery
    {
        private readonly string _includeProperties = $"{nameof(LandTitle)},{nameof(Locality)}";
        public ParcelQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override IEnumerable<ParcelDto> GetAll()
        {
            var parcels = _unitOfWork.Parcel.GetAll(includeProperties: _includeProperties)
                .OrderBy(u => u.Number);

            return MapEntitiesToDto(parcels);
        }

        public override ParcelDto GetById(int parcelId)
        {
            var parcel = _unitOfWork.Parcel.GetAll(s => s.Id == parcelId,
                includeProperties: _includeProperties).ToList().FirstOrDefault();

            return MapEntityToDto(parcel);
        }

        public IEnumerable<ParcelDto> GetByLandTitleId(int landTitleId)
        {
            var parcels = _unitOfWork.Parcel.GetAll(s => s.LandTitleId == landTitleId,
                includeProperties: _includeProperties).ToList();

            return MapEntitiesToDto(parcels);
        }

        public IEnumerable<ParcelDto> GetByStatus(string status)
        {
            var parcels = _unitOfWork.Parcel.GetAll(s => s.Status.ToLower() == status.ToLower(),
                includeProperties: _includeProperties)
               .OrderBy(u => u.Number);
            
            return MapEntitiesToDto(parcels);
        }
    }
}
