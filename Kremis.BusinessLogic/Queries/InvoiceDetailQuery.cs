using AutoMapper;
using Kremis.Domain.Entities;
using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;

namespace Kremis.BusinessLogic.Queries
{
    public class InvoiceDetailQuery : BaseQuery<InvoiceDetailDto, InvoiceDetail, int>, IInvoiceDetailQuery
    {
        private readonly string _includeProperties = $"{nameof(InvoiceHeader)},{nameof(Parcel)}";
        public InvoiceDetailQuery(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override IEnumerable<InvoiceDetailDto> GetAll()
        {
            var invoiceDetails = _unitOfWork.InvoiceDetail.GetAll(includeProperties: _includeProperties); 
            return MapEntitiesToDto(invoiceDetails);
        }

        public override InvoiceDetailDto GetById(int invoiceDetailId)
        {
            var invoiceDetail = _unitOfWork.InvoiceDetail.GetAll(u => u.Id == invoiceDetailId,
                includeProperties: _includeProperties).FirstOrDefault();
            if (invoiceDetail != null && invoiceDetail.Parcel != null)
            {
                invoiceDetail.Parcel.LandTitle = _unitOfWork.LandTitle.GetAll(u => u.Id == invoiceDetail.Parcel.LandTitleId,
                includeProperties: $"{nameof(Locality)}").FirstOrDefault();
            }
            return MapEntityToDto(invoiceDetail);
        }


        public IEnumerable<InvoiceDetailDto> GetByInvoiceHeaderId(int invoiceHeaderId)
        {
            var invoiceDetails = _unitOfWork.InvoiceDetail.GetAll(u => u.InvoiceHeaderId == invoiceHeaderId,
                includeProperties: _includeProperties).ToList();

            foreach (var invoiceDetail in invoiceDetails)
            {
                if (invoiceDetail != null && invoiceDetail.Parcel != null)
                {
                    invoiceDetail.Parcel.LandTitle = _unitOfWork.LandTitle.GetAll(u => u.Id == invoiceDetail.Parcel.LandTitleId,
                    includeProperties: $"{nameof(Locality)}").FirstOrDefault();
                }
            }
            return MapEntitiesToDto(invoiceDetails);
        }

        public InvoiceDetailDto GetByParcelId(int parcelId)
        {
            var invoiceDetail = _unitOfWork.InvoiceDetail.GetAll(u => u.ParcelId == parcelId,
                includeProperties: _includeProperties).FirstOrDefault();

            if (invoiceDetail != null && invoiceDetail.Parcel != null)
            {
                invoiceDetail.Parcel.LandTitle = _unitOfWork.LandTitle.GetAll(u => u.Id == invoiceDetail.Parcel.LandTitleId,
                includeProperties: $"{nameof(Locality)}").FirstOrDefault();
            }

            return MapEntityToDto(invoiceDetail);
        }
    }
}
