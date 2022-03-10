using AutoMapper;
using Kremis.Domain.Assemblers;
using Kremis.Domain.Entities;

namespace Kremis.Mvc.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<City, CityDto>();
            CreateMap<CityDto, City>();

            CreateMap<PaymentMode, PaymentModeDto>();
            CreateMap<PaymentModeDto, PaymentMode>();

            CreateMap<Locality, LocalityDto>();
            CreateMap<LocalityDto, Locality>();

            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();

            CreateMap<Parcel, ParcelDto>();
            CreateMap<ParcelDto, Parcel>();

            CreateMap<LandTitle, LandTitleDto>();
            CreateMap<LandTitleDto, LandTitle>();

            CreateMap<BusinessPartner, BusinessPartnerDto>();
            CreateMap<BusinessPartnerDto, BusinessPartner>();

            CreateMap<DocumentType, DocumentTypeDto>();
            CreateMap<DocumentTypeDto, DocumentType>();

            CreateMap<LandTitleDocument, LandTitleDocumentDto>();
            CreateMap<LandTitleDocumentDto, LandTitleDocument>();

            CreateMap<CustomerDocument, CustomerDocumentDto>();
            CreateMap<CustomerDocumentDto, CustomerDocument>();

            CreateMap<IdentityDocumentType, IdentityDocumentTypeDto>();
            CreateMap<IdentityDocumentTypeDto, IdentityDocumentType>();

            CreateMap<InvoiceHeader, InvoiceHeaderDto>();
            CreateMap<InvoiceHeaderDto, InvoiceHeader>();

            CreateMap<InvoiceDetail, InvoiceDetailDto>();
            CreateMap<InvoiceDetailDto, InvoiceDetail>();

            CreateMap<InvoicePayment, InvoicePaymentDto>();
            CreateMap<InvoicePaymentDto, InvoicePayment>();

            CreateMap<CommissionPayment, CommissionPaymentDto>();
            CreateMap<CommissionPaymentDto, CommissionPayment>();

            CreateMap<ParcelDocument, ParcelDocumentDto>();
            CreateMap<ParcelDocumentDto, ParcelDocument>();
        }
    }
}
