using Kremis.Domain.Entities;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class PaymentModeRepository : BaseRepository<PaymentMode, int>, IPaymentModeRepository
    {
        public PaymentModeRepository(ApplicationDbContext dbContext) : base(dbContext) 
        { 
        }

        public virtual void Update(PaymentMode cityToUpdate)
        {
            var originalEntity = GetById(cityToUpdate.Id);

            if (!string.IsNullOrWhiteSpace(cityToUpdate.Name)) originalEntity.Name = cityToUpdate.Name;
            originalEntity.LastModificationDate = cityToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = cityToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
