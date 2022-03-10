
namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customer { get; }
        ICustomerDocumentRepository CustomerDocument { get; }
        IParcelRepository Parcel { get; }
        IParcelDocumentRepository ParcelDocument { get; }
        ILandTitleRepository LandTitle { get; }
        IBusinessPartnerRepository BusinessPartner { get; }
        IApplicationUserRepository ApplicationUser { get; }
        ICityRepository City { get; }
        IDocumentTypeRepository DocumentType { get; }
        ILandTitleDocumentRepository LandTitleDocument { get; }
        IIdentityDocumentTypeRepository IdentityDocumentType { get; }
        IInvoiceHeaderRepository InvoiceHeader { get; }
        IInvoiceDetailRepository InvoiceDetail { get; }
        IInvoicePaymentRepository InvoicePayment { get;  }
        ICommissionPaymentRepository CommissionPayment { get; }
        ILocalityRepository Locality { get; }
        IPaymentModeRepository PaymentMode { get; }

        void Save();
    }

}
