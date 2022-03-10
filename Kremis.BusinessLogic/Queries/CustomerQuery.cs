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
    public class CustomerQuery : BaseQuery<CustomerDto, Customer, int>, ICustomerQuery
    {
        private string _includeProperties = $"{nameof(IdentityDocumentType)}";
        public CustomerQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override IEnumerable<CustomerDto> GetAll()
        {
            var customers = _unitOfWork.Customer.GetAll(includeProperties: _includeProperties)
                .OrderBy(u => u.Name);
            return MapEntitiesToDto(customers);
        }

        public override CustomerDto GetById(int customerId)
        {
            var customer = _unitOfWork.Customer.GetAll(u => u.Id == customerId,
                includeProperties: _includeProperties).FirstOrDefault();
            return MapEntityToDto(customer);
        }
    }
}
