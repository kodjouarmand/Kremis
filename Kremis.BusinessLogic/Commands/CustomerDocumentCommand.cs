using AutoMapper;
using Kremis.Domain.Entities;
using Kremis.BusinessLogic.Exceptions;
using System;
using System.Text;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Utility.Helpers;
using Kremis.Utility.Enum;
using Microsoft.AspNetCore.Hosting;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class CustomerDocumentCommand : BaseCommand<CustomerDocumentDto, CustomerDocument, int>, ICustomerDocumentCommand
    {
        private readonly ICustomerDocumentQuery _customerDocumentQuery;
        private readonly IWebHostEnvironment _hostEnvironment;
        public CustomerDocumentCommand(IUnitOfWork unitOfWork, IMapper mapper,
            ICustomerDocumentQuery customerDocumentQuery, IWebHostEnvironment hostEnvironment) : base(unitOfWork, mapper)
        {
            _customerDocumentQuery = customerDocumentQuery;
            _hostEnvironment = hostEnvironment;
        }

        protected override StringBuilder ValidateAdd(CustomerDocumentDto customerDocumentDto)
        {
            StringBuilder validationErrors = new();

            if (!customerDocumentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new CustomerDocumentValidator().Validate(customerDocumentDto);
            validationErrors.Append(validationResult.ToString());

            if (_customerDocumentQuery.GetByDocumentUrl(customerDocumentDto.DocumentUrl) != null)
            {
                validationErrors.Append("Un document avec ce nom existe déjà;\n");
                return validationErrors;
            }
            return validationErrors;
        }

        public override int Add(CustomerDocumentDto customerDocumentDto)
        {
            var customerDocument = BuildEntity(customerDocumentDto);
            var customerDocumentId = _unitOfWork.CustomerDocument.Add(customerDocument);
            FileHelper.CreateFile(customerDocumentDto.Document, DocumentOwnerEnum.Customer, _hostEnvironment.WebRootPath);
            return customerDocumentId;
        }

        protected override StringBuilder ValidateUpdate(CustomerDocumentDto customerDocumentDto)
        {
            StringBuilder validationErrors = new();

            if (customerDocumentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new CustomerDocumentValidator().Validate(customerDocumentDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(CustomerDocumentDto customerDocumentDto)
        {
            CustomerDocumentDto originalCustomerDocumentDto = _customerDocumentQuery.GetById(customerDocumentDto.Id);
            if (customerDocumentDto.Document == null)
            {
                customerDocumentDto.DocumentUrl = originalCustomerDocumentDto.DocumentUrl;
            }

            var customerDocument = BuildEntity(customerDocumentDto);
            _unitOfWork.CustomerDocument.Update(customerDocument);
            FileHelper.CreateFile(customerDocumentDto.Document, DocumentOwnerEnum.Customer, _hostEnvironment.WebRootPath,
                originalCustomerDocumentDto.DocumentUrl);
        }

        protected override StringBuilder ValidateDelete(CustomerDocumentDto customerDocumentDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(customerDocumentDto);
            return validationErrors;
        }

        public override void Delete(int customerDocumentId)
        {
            var customerDocumentDto = _customerDocumentQuery.GetById(customerDocumentId);
            StringBuilder validationErrors = ValidateDelete(customerDocumentDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.CustomerDocument.Delete(customerDocumentId);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }
    }
}
