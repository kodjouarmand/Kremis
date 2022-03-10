using AutoMapper;
using Kremis.Domain.Entities;
using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;

namespace Kremis.BusinessLogic.Queries
{
    public class LocalityQuery : BaseQuery<LocalityDto, Locality, int>, ILocalityQuery
    {
        private string _includeProperties = $"{nameof(City)}";
        public LocalityQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) 
        {
        }

        public override IEnumerable<LocalityDto> GetAll()
        {
            var localitys = _unitOfWork.Locality.GetAll(includeProperties: _includeProperties)
                .OrderBy(u => u.Name);
            return MapEntitiesToDto(localitys);
        }

        public override LocalityDto GetById(int localityId)
        {
            var locality = _unitOfWork.Locality.GetAll(u => u.Id == localityId, 
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(locality);
        }

        public LocalityDto GetByNumber(string number)
        {
            var localitys = _unitOfWork.Locality.GetAll(u => u.Name == number,
                includeProperties: $"{_includeProperties}").FirstOrDefault();
            return MapEntityToDto(localitys);

        }

        public IEnumerable<LocalityDto> GetByCityId(int cityId)
        {
            var localitys = _unitOfWork.Locality.GetAll(u => u.CityId == cityId,
                includeProperties: $"{_includeProperties}")
                .OrderBy(u => u.Name);
            return MapEntitiesToDto(localitys);
        }
    }
}
