using AutoMapper;
using Kremis.Domain.Entities;
using System;
using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;
using System.Text;

namespace Kremis.BusinessLogic.Queries
{
    public class LandTitleDocumentQuery : BaseQuery<LandTitleDocumentDto, LandTitleDocument, int>, ILandTitleDocumentQuery
    {
        private string _includeProperties = $"{nameof(LandTitle)},{nameof(DocumentType)}";
        public LandTitleDocumentQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) 
        {
        }

        public override IEnumerable<LandTitleDocumentDto> GetAll()
        {
            var landTitleDocuments = _unitOfWork.LandTitleDocument.GetAll(includeProperties: _includeProperties)
                .OrderBy(u => u.DocumentUrl);
            return MapEntitiesToDto(landTitleDocuments);
        }

        public override LandTitleDocumentDto GetById(int landTitleDocumentId)
        {
            var landTitleDocument = _unitOfWork.LandTitleDocument.GetAll(u => u.Id == landTitleDocumentId, 
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(landTitleDocument);
        }


        public IEnumerable<LandTitleDocumentDto> GetByLandTitleId(int landTitleId)
        {
            var landTitleDocuments = _unitOfWork.LandTitleDocument.GetAll(u => u.LandTitleId == landTitleId,
                includeProperties: _includeProperties).ToList();
            return MapEntitiesToDto(landTitleDocuments);
        }

        public LandTitleDocumentDto GetByDocumentUrl(string documentUrld)
        {
            var landTitleDocument = _unitOfWork.LandTitleDocument.GetAll(u => u.DocumentUrl == documentUrld,
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(landTitleDocument);
        }
    }
}
