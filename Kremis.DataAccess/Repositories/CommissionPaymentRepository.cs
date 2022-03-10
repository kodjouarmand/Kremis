using Kremis.Domain.Entities;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class CommissionPaymentRepository : BaseRepository<CommissionPayment, int>, ICommissionPaymentRepository
    {
        public CommissionPaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public virtual void Update(CommissionPayment commissionPaymentToUpdate)
        {
            var originalEntity = GetById(commissionPaymentToUpdate.Id);
            originalEntity.InvoiceHeaderId = commissionPaymentToUpdate.InvoiceHeaderId;
            originalEntity.PaymentModeId = commissionPaymentToUpdate.PaymentModeId;
            originalEntity.AmountPaid = commissionPaymentToUpdate.AmountPaid;
            if (commissionPaymentToUpdate.Date != default) originalEntity.Date = commissionPaymentToUpdate.Date;
            if (!string.IsNullOrWhiteSpace(commissionPaymentToUpdate.TransactionNumber)) originalEntity.TransactionNumber = commissionPaymentToUpdate.TransactionNumber;
            originalEntity.IsCanceled = commissionPaymentToUpdate.IsCanceled;
            if (!string.IsNullOrWhiteSpace(commissionPaymentToUpdate.CancelationReason)) originalEntity.CancelationReason = commissionPaymentToUpdate.CancelationReason;
            originalEntity.LastModificationDate = commissionPaymentToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = commissionPaymentToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
