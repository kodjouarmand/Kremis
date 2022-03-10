using AutoMapper;
using Kremis.Domain.Entities;
using Kremis.BusinessLogic.Exceptions;
using System.Text;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class BusinessPartnerCommand : BaseCommand<BusinessPartnerDto, BusinessPartner, int>, IBusinessPartnerCommand
    {
        private readonly IBusinessPartnerQuery _businessPartnerQuery;
        public BusinessPartnerCommand(IUnitOfWork unitOfWork, IMapper mapper,
            IBusinessPartnerQuery businessPartnerQuery) : base(unitOfWork, mapper)
        {
            _businessPartnerQuery = businessPartnerQuery;
        }

        protected override StringBuilder ValidateAdd(BusinessPartnerDto businessPartnerDto)
        {
            StringBuilder validationErrors = new();

            if (!businessPartnerDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new BusinessPartnerValidator().Validate(businessPartnerDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override int Add(BusinessPartnerDto businessPartnerDto)
        {
            var businessPartner = BuildEntity(businessPartnerDto);
            _unitOfWork.BusinessPartner.Add(businessPartner);
            _unitOfWork.Save();
            return businessPartner.Id;
        }

        protected override StringBuilder ValidateUpdate(BusinessPartnerDto businessPartnerDto)
        {
            StringBuilder validationErrors = new();

            if (businessPartnerDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new BusinessPartnerValidator().Validate(businessPartnerDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(BusinessPartnerDto businessPartnerDto)
        {
            var businessPartner = BuildEntity(businessPartnerDto);
            _unitOfWork.BusinessPartner.Update(businessPartner);
        }

        protected override StringBuilder ValidateDelete(BusinessPartnerDto businessPartnerDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(businessPartnerDto);
            return validationErrors;
        }

        public override void Delete(int businessPartnerId)
        {
            var businessPartnerDto = _businessPartnerQuery.GetById(businessPartnerId);
            StringBuilder validationErrors = ValidateDelete(businessPartnerDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.BusinessPartner.Delete(businessPartnerId);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }
    }
}
