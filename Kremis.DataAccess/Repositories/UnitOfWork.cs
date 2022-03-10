using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;
using System;

namespace Kremis.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public ICustomerRepository Customer { get; private set; }
        public ICustomerDocumentRepository CustomerDocument { get; private set; }
        public IParcelRepository Parcel { get; private set; }
        public IParcelDocumentRepository ParcelDocument { get; private set; }
        public ILandTitleRepository LandTitle { get; private set; }
        public IBusinessPartnerRepository BusinessPartner { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ICityRepository City { get; private set; }
        public IDocumentTypeRepository DocumentType { get; private set; }
        public ILandTitleDocumentRepository LandTitleDocument { get; private set; }
        public IIdentityDocumentTypeRepository IdentityDocumentType { get; private set; }
        public IInvoiceHeaderRepository InvoiceHeader{ get; private set; }
        public IInvoiceDetailRepository InvoiceDetail { get; private set; }
        public IInvoicePaymentRepository InvoicePayment { get; private set; }
        public ICommissionPaymentRepository CommissionPayment { get; private set; }
        public ILocalityRepository Locality { get; private set; }
        public IPaymentModeRepository PaymentMode { get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Customer = new CustomerRepository(_dbContext);
            CustomerDocument = new CustomerDocumentRepository(_dbContext);
            Parcel = new ParcelRepository(_dbContext);
            ParcelDocument = new ParcelDocumentRepository(_dbContext);
            LandTitle = new LandTitleRepository(_dbContext);
            BusinessPartner = new BusinessPartnerRepository(_dbContext);
            ApplicationUser = new ApplicationUserRepository(_dbContext);
            City = new CityRepository(_dbContext);
            DocumentType = new DocumentTypeRepository(_dbContext);
            LandTitleDocument = new LandTitleDocumentRepository(_dbContext);
            IdentityDocumentType = new IdentityDocumentTypeRepository(_dbContext);
            InvoiceHeader = new InvoiceHeaderRepository(_dbContext);
            InvoiceDetail = new InvoiceDetailRepository(_dbContext);
            InvoicePayment = new InvoicePaymentRepository(_dbContext);
            CommissionPayment = new CommissionPaymentRepository(_dbContext);
            Locality = new LocalityRepository(_dbContext);
            PaymentMode = new PaymentModeRepository(_dbContext);
        }
        

        public void Dispose() => _dbContext.Dispose();

        public void Save() => _dbContext.SaveChanges();
    }
}
