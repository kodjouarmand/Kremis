using Kremis.Domain.Entities;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class CustomerDocumentRepository : BaseRepository<CustomerDocument, int>, ICustomerDocumentRepository
    {
        public CustomerDocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public virtual void Update(CustomerDocument customerDocumentToUpdate)
        {
            var originalEntity = GetById(customerDocumentToUpdate.Id);

            originalEntity.DocumentTypeId = customerDocumentToUpdate.DocumentTypeId;
            originalEntity.CustomerId = customerDocumentToUpdate.CustomerId;
            if (!string.IsNullOrWhiteSpace(customerDocumentToUpdate.DocumentUrl)) originalEntity.DocumentUrl = customerDocumentToUpdate.DocumentUrl;
            originalEntity.LastModificationDate = customerDocumentToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = customerDocumentToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
