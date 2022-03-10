using AutoMapper;
using Kremis.Domain.Entities;
using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;

namespace Kremis.BusinessLogic.Queries
{
    public class CommissionPaymentQuery : BaseQuery<CommissionPaymentDto, CommissionPayment, int>, ICommissionPaymentQuery
    {
        private string _includeProperties = $"{nameof(InvoiceHeader)},{nameof(PaymentMode)}";
        public CommissionPaymentQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override IEnumerable<CommissionPaymentDto> GetAll()
        {
            var commissionPayments = _unitOfWork.CommissionPayment.GetAll(includeProperties: _includeProperties)
                .OrderByDescending(u => u.Date);
            foreach (var commissionPayment in commissionPayments)
            {
                if (commissionPayment.InvoiceHeader.BusinessPartnerId.HasValue)
                {
                    var businessPartner = _unitOfWork.BusinessPartner.GetById(commissionPayment.InvoiceHeader.BusinessPartnerId.GetValueOrDefault());
                    commissionPayment.InvoiceHeader.BusinessPartner = businessPartner;
                }
            }
            return MapEntitiesToDto(commissionPayments);
        }

        public override CommissionPaymentDto GetById(int commissionPaymentId)
        {
            var commissionPayment = _unitOfWork.CommissionPayment.GetAll(u => u.Id == commissionPaymentId,
                includeProperties: _includeProperties).FirstOrDefault();

            if (commissionPayment != null && commissionPayment.InvoiceHeader.BusinessPartnerId.HasValue)
            {
                var businessPartner = _unitOfWork.BusinessPartner.GetById(commissionPayment.InvoiceHeader.BusinessPartnerId.GetValueOrDefault());
                commissionPayment.InvoiceHeader.BusinessPartner = businessPartner;
            }
            return MapEntityToDto(commissionPayment);
        }


        public IEnumerable<CommissionPaymentDto> GetByInvoiceHeaderId(int invoiceHeaderId)
        {
            var commissionPayments = _unitOfWork.CommissionPayment.GetAll(u => u.InvoiceHeaderId == invoiceHeaderId,
                includeProperties: _includeProperties).ToList();
            foreach (var commissionPayment in commissionPayments)
            {
                if (commissionPayment.InvoiceHeader.BusinessPartnerId.HasValue)
                {
                    var businessPartner = _unitOfWork.BusinessPartner.GetById(commissionPayment.InvoiceHeader.BusinessPartnerId.GetValueOrDefault());
                    commissionPayment.InvoiceHeader.BusinessPartner = businessPartner;
                }
            }
            return MapEntitiesToDto(commissionPayments);
        }

    }
}
