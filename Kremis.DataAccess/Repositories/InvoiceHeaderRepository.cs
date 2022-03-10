using Kremis.Domain.Entities;
using System;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class InvoiceHeaderRepository : BaseRepository<InvoiceHeader, int>, IInvoiceHeaderRepository
    {
        public InvoiceHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public virtual void Update(InvoiceHeader invoiceHeaderToUpdate)
        {
            var originalEntity = GetById(invoiceHeaderToUpdate.Id);
            originalEntity.CustomerId = invoiceHeaderToUpdate.CustomerId;
            if (invoiceHeaderToUpdate.BusinessPartnerId != null) originalEntity.BusinessPartnerId = invoiceHeaderToUpdate.BusinessPartnerId;
            if (invoiceHeaderToUpdate.Date != default) originalEntity.Date = invoiceHeaderToUpdate.Date;
            if (invoiceHeaderToUpdate.PaymentDueDate != default) originalEntity.PaymentDueDate = invoiceHeaderToUpdate.PaymentDueDate;
            originalEntity.ParcellingCosts = invoiceHeaderToUpdate.ParcellingCosts;
            originalEntity.TechnicalFileCosts = invoiceHeaderToUpdate.TechnicalFileCosts;
            originalEntity.BoundaryCosts = invoiceHeaderToUpdate.BoundaryCosts;
            originalEntity.TotalSaleAmount = invoiceHeaderToUpdate.TotalSaleAmount;
            originalEntity.NetAmountToPay = invoiceHeaderToUpdate.NetAmountToPay;
            originalEntity.AdvancedAmount = invoiceHeaderToUpdate.AdvancedAmount;
            originalEntity.RemainingAmountToPay = invoiceHeaderToUpdate.RemainingAmountToPay;
            if (!string.IsNullOrWhiteSpace(invoiceHeaderToUpdate.Status)) originalEntity.Status = invoiceHeaderToUpdate.Status;
            if (!string.IsNullOrWhiteSpace(invoiceHeaderToUpdate.CommissionType)) originalEntity.CommissionType = invoiceHeaderToUpdate.CommissionType;
            originalEntity.CommissionUnitValue = invoiceHeaderToUpdate.CommissionUnitValue;
            originalEntity.CommissionToPay = invoiceHeaderToUpdate.CommissionToPay;
            if (!string.IsNullOrWhiteSpace(invoiceHeaderToUpdate.CommissionStatus)) originalEntity.CommissionStatus = invoiceHeaderToUpdate.CommissionStatus;
            if (!string.IsNullOrWhiteSpace(invoiceHeaderToUpdate.Comment)) originalEntity.Comment = invoiceHeaderToUpdate.Comment;
            originalEntity.IsCanceled = invoiceHeaderToUpdate.IsCanceled;
            if (!string.IsNullOrWhiteSpace(invoiceHeaderToUpdate.CancelationReason)) originalEntity.CancelationReason = invoiceHeaderToUpdate.CancelationReason;
            originalEntity.LastModificationDate = invoiceHeaderToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = invoiceHeaderToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }
}
