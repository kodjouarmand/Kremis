using Kremis.Domain.Entities;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class DocumentTypeRepository : BaseRepository<DocumentType, int>, IDocumentTypeRepository
    {
        public DocumentTypeRepository(ApplicationDbContext dbContext) : base(dbContext) 
        { 
        }

        public virtual void Update(DocumentType documentTypeToUpdate)
        {
            var originalEntity = GetById(documentTypeToUpdate.Id);

            if (!string.IsNullOrWhiteSpace(documentTypeToUpdate.Name)) originalEntity.Name = documentTypeToUpdate.Name;
            originalEntity.LastModificationDate = documentTypeToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = documentTypeToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
