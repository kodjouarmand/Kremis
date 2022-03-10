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
    public class DocumentTypeQuery : BaseQuery<DocumentTypeDto, DocumentType, int>, IDocumentTypeQuery
    {
        public DocumentTypeQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public override IEnumerable<DocumentTypeDto> GetAll()
        {
            var companies = _unitOfWork.DocumentType.GetAll()
                .OrderBy(c => c.Name);
            return MapEntitiesToDto(companies);
        }

        public override DocumentTypeDto GetById(int documentTypeId)
        {
            var documentType = _unitOfWork.DocumentType.GetById(documentTypeId);
            return MapEntityToDto(documentType);
        }
    }
}
