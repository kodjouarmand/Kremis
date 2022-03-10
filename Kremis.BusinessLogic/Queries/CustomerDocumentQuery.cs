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
    public class CustomerDocumentQuery : BaseQuery<CustomerDocumentDto, CustomerDocument, int>, ICustomerDocumentQuery
    {
        private readonly string _includeProperties = $"{nameof(Customer)},{nameof(DocumentType)}";
        public CustomerDocumentQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) 
        {
        }

        public override IEnumerable<CustomerDocumentDto> GetAll()
        {
            var customerDocuments = _unitOfWork.CustomerDocument.GetAll(includeProperties: _includeProperties)
                .OrderBy(u => u.DocumentUrl);
            return MapEntitiesToDto(customerDocuments);
        }

        public override CustomerDocumentDto GetById(int customerDocumentId)
        {
            var customerDocument = _unitOfWork.CustomerDocument.GetAll(u => u.Id == customerDocumentId, 
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(customerDocument);
        }


        public IEnumerable<CustomerDocumentDto> GetByCustomerId(int customerId)
        {
            var customerDocuments = _unitOfWork.CustomerDocument.GetAll(u => u.CustomerId == customerId,
                includeProperties: _includeProperties).ToList();
            return MapEntitiesToDto(customerDocuments);
        }

        public CustomerDocumentDto GetByDocumentUrl(string documentUrld)
        {
            var customerDocument = _unitOfWork.CustomerDocument.GetAll(u => u.DocumentUrl == documentUrld,
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(customerDocument);
        }
    }
}
