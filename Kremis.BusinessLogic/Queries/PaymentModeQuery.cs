using AutoMapper;
using Kremis.Domain.Entities;
using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;

namespace Kremis.BusinessLogic.Queries
{
    public class PaymentModeQuery : BaseQuery<PaymentModeDto, PaymentMode, int>, IPaymentModeQuery
    {
        public PaymentModeQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public override IEnumerable<PaymentModeDto> GetAll()
        {
            var cities = _unitOfWork.PaymentMode.GetAll()
                .OrderBy(c => c.Name);
            return MapEntitiesToDto(cities);
        }

        public override PaymentModeDto GetById(int cityId)
        {
            var city = _unitOfWork.PaymentMode.GetById(cityId);
            return MapEntityToDto(city);
        }
    }
}
