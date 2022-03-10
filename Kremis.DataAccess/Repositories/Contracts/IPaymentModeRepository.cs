using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IPaymentModeRepository : IBaseRepository<PaymentMode, int>
    {
        public void Update(PaymentMode city);        
    }
}
