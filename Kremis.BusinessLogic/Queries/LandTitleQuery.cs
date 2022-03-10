using AutoMapper;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;
using System.Text;
using Kremis.Utility.Helpers;

namespace Kremis.BusinessLogic.Queries
{
    public class LandTitleQuery : BaseQuery<LandTitleDto, LandTitle, int>, ILandTitleQuery
    {
        private string _includeProperties = $"{nameof(Locality)}";
        public LandTitleQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) 
        {
        }

        public override IEnumerable<LandTitleDto> GetAll()
        {
            var landTitles = _unitOfWork.LandTitle.GetAll(includeProperties: _includeProperties)
                .OrderBy(u => u.Number);
            return MapEntitiesToDto(landTitles);
        }

        public override LandTitleDto GetById(int landTitleId)
        {
            var landTitle = _unitOfWork.LandTitle.GetAll(u => u.Id == landTitleId, 
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(landTitle);
        }

        public LandTitleDto GetByNumber(string number)
        {
            var landTitles = _unitOfWork.LandTitle.GetAll(u => u.Number == number,
                includeProperties: $"{_includeProperties}").FirstOrDefault();
            return MapEntityToDto(landTitles);

        }

        public IEnumerable<LandTitleDto> GetByLocalityId(int localityId)
        {
            var landTitles = _unitOfWork.LandTitle.GetAll(u => u.LocalityId == localityId,
                includeProperties: $"{_includeProperties}")
                .OrderBy(u => u.Number);
            return MapEntitiesToDto(landTitles);
        }
    }
}
