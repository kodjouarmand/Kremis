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
    public class CustomerCommand : BaseCommand<CustomerDto, Customer, int>, ICustomerCommand
    {
        private readonly ICustomerQuery _customerQuery;
        public CustomerCommand(IUnitOfWork unitOfWork, IMapper mapper,
            ICustomerQuery customerQuery) : base(unitOfWork, mapper)
        {
            _customerQuery = customerQuery;
        }

        protected override StringBuilder ValidateAdd(CustomerDto customerDto)
        {
            StringBuilder validationErrors = new();

            if (!customerDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new CustomerValidator().Validate(customerDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override int Add(CustomerDto customerDto)
        {
            var customer = BuildEntity(customerDto);
            _unitOfWork.Customer.Add(customer);
            _unitOfWork.Save();
            return customer.Id;
        }

        protected override StringBuilder ValidateUpdate(CustomerDto customerDto)
        {
            StringBuilder validationErrors = new();

            if (customerDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new CustomerValidator().Validate(customerDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(CustomerDto customerDto)
        {
            var customer = BuildEntity(customerDto);
            _unitOfWork.Customer.Update(customer);
        }

        protected override StringBuilder ValidateDelete(CustomerDto customerDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(customerDto);
            return validationErrors;
        }

        public override void Delete(int customerId)
        {
            var customerDto = _customerQuery.GetById(customerId);
            StringBuilder validationErrors = ValidateDelete(customerDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.Customer.Delete(customerId);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }
    }
}
