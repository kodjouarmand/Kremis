using Kremis.Domain.Entities;
using System;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class BusinessPartnerRepository : BaseRepository<BusinessPartner, int>, IBusinessPartnerRepository
    {
        public BusinessPartnerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public virtual void Update(BusinessPartner businessPartnerToUpdate)
        {
            var originalEntity = GetById(businessPartnerToUpdate.Id);

            if (!string.IsNullOrWhiteSpace(businessPartnerToUpdate.Name)) originalEntity.Name = businessPartnerToUpdate.Name;
            if (!string.IsNullOrWhiteSpace(businessPartnerToUpdate.PhoneNumber)) originalEntity.PhoneNumber = businessPartnerToUpdate.PhoneNumber;
            if (businessPartnerToUpdate.AccountBalance != default) originalEntity.AccountBalance = businessPartnerToUpdate.AccountBalance;
            if (!string.IsNullOrWhiteSpace(businessPartnerToUpdate.Comment)) originalEntity.Comment = businessPartnerToUpdate.Comment;
            originalEntity.LastModificationDate = businessPartnerToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = businessPartnerToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
