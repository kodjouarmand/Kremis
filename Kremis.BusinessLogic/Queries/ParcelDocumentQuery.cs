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
    public class ParcelDocumentQuery : BaseQuery<ParcelDocumentDto, ParcelDocument, int>, IParcelDocumentQuery
    {
        private readonly string _includeProperties = $"{nameof(Parcel)},{nameof(DocumentType)}";
        public ParcelDocumentQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) 
        {
        }

        public override IEnumerable<ParcelDocumentDto> GetAll()
        {
            var parcelDocuments = _unitOfWork.ParcelDocument.GetAll(includeProperties: _includeProperties)
                .OrderBy(u => u.DocumentUrl);
            return MapEntitiesToDto(parcelDocuments);
        }

        public override ParcelDocumentDto GetById(int parcelDocumentId)
        {
            var parcelDocument = _unitOfWork.ParcelDocument.GetAll(u => u.Id == parcelDocumentId, 
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(parcelDocument);
        }


        public IEnumerable<ParcelDocumentDto> GetByParcelId(int parcelId)
        {
            var parcelDocuments = _unitOfWork.ParcelDocument.GetAll(u => u.ParcelId == parcelId,
                includeProperties: _includeProperties).ToList();
            return MapEntitiesToDto(parcelDocuments);
        }

        public ParcelDocumentDto GetByDocumentUrl(string documentUrld)
        {
            var parcelDocument = _unitOfWork.ParcelDocument.GetAll(u => u.DocumentUrl == documentUrld,
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(parcelDocument);
        }
    }
}
