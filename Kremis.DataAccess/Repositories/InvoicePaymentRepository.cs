using Kremis.Domain.Entities;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class InvoicePaymentRepository : BaseRepository<InvoicePayment, int>, IInvoicePaymentRepository
    {
        public InvoicePaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public virtual void Update(InvoicePayment invoicePaymentToUpdate)
        {
            var originalEntity = GetById(invoicePaymentToUpdate.Id);
            originalEntity.InvoiceHeaderId = invoicePaymentToUpdate.InvoiceHeaderId;
            originalEntity.PaymentModeId = invoicePaymentToUpdate.PaymentModeId;
            originalEntity.AmountPaid = invoicePaymentToUpdate.AmountPaid;
            if (!string.IsNullOrWhiteSpace(invoicePaymentToUpdate.TransactionNumber)) originalEntity.TransactionNumber = invoicePaymentToUpdate.TransactionNumber;
            originalEntity.IsCanceled = invoicePaymentToUpdate.IsCanceled;
            if (!string.IsNullOrWhiteSpace(invoicePaymentToUpdate.CancelationReason)) originalEntity.CancelationReason = invoicePaymentToUpdate.CancelationReason;
            if (invoicePaymentToUpdate.Date != default) originalEntity.Date = invoicePaymentToUpdate.Date;
            originalEntity.LastModificationDate = invoicePaymentToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = invoicePaymentToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
