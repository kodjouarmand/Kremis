using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface ICommissionPaymentRepository : IBaseRepository<CommissionPayment, int>
    {
        public void Update(CommissionPayment commissionPayment);
    }
}
