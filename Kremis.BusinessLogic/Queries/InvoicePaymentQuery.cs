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
    public class InvoicePaymentQuery : BaseQuery<InvoicePaymentDto, InvoicePayment, int>, IInvoicePaymentQuery
    {
        private string _includeProperties = $"{nameof(InvoiceHeader)},{nameof(PaymentMode)}";
        public InvoicePaymentQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override IEnumerable<InvoicePaymentDto> GetAll()
        {
            var invoicePayments = _unitOfWork.InvoicePayment.GetAll(includeProperties: _includeProperties)
                .OrderByDescending(u => u.Date);
            foreach (var invoicePayment in invoicePayments)
            {
                var customer = _unitOfWork.Customer.GetById(invoicePayment.InvoiceHeader.CustomerId);
                invoicePayment.InvoiceHeader.Customer = customer;
            }
            return MapEntitiesToDto(invoicePayments);
        }

        public override InvoicePaymentDto GetById(int invoicePaymentId)
        {
            var invoicePayment = _unitOfWork.InvoicePayment.GetAll(u => u.Id == invoicePaymentId,
                includeProperties: _includeProperties).FirstOrDefault();

            if (invoicePayment != null)
            {
                var customer = _unitOfWork.Customer.GetById(invoicePayment.InvoiceHeader.CustomerId);
                invoicePayment.InvoiceHeader.Customer = customer;
            }
            return MapEntityToDto(invoicePayment);
        }


        public IEnumerable<InvoicePaymentDto> GetByInvoiceHeaderId(int invoiceHeaderId)
        {
            var invoicePayments = _unitOfWork.InvoicePayment.GetAll(u => u.InvoiceHeaderId == invoiceHeaderId,
                includeProperties: _includeProperties).ToList();
            foreach (var invoicePayment in invoicePayments)
            {
                var customer = _unitOfWork.Customer.GetById(invoicePayment.InvoiceHeader.CustomerId);
                invoicePayment.InvoiceHeader.Customer = customer;
            }
            return MapEntitiesToDto(invoicePayments);
        }

    }
}
