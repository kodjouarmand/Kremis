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
    public class IdentityDocumentTypeQuery : BaseQuery<IdentityDocumentTypeDto, IdentityDocumentType, int>, IIdentityDocumentTypeQuery
    {
        public IdentityDocumentTypeQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public override IEnumerable<IdentityDocumentTypeDto> GetAll()
        {
            var companies = _unitOfWork.IdentityDocumentType.GetAll()
                .OrderBy(c => c.Name);
            return MapEntitiesToDto(companies);
        }

        public override IdentityDocumentTypeDto GetById(int cityId)
        {
            var city = _unitOfWork.IdentityDocumentType.GetById(cityId);
            return MapEntityToDto(city);
        }
    }
}
