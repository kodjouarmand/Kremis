using Kremis.Domain.Entities;
using System;
using Kremis.Domain.Contexts;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.DataAccess.Repositories
{
    public class CustomerRepository : BaseRepository<Customer, int>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public virtual void Update(Customer customerToUpdate)
        {
            var originalEntity = GetById(customerToUpdate.Id);

            if (customerToUpdate.IdentityDocumentTypeId != default) originalEntity.IdentityDocumentTypeId = customerToUpdate.IdentityDocumentTypeId;
            if (!string.IsNullOrWhiteSpace(customerToUpdate.Name)) originalEntity.Name = customerToUpdate.Name;
            if (!string.IsNullOrWhiteSpace(customerToUpdate.PhoneNumber)) originalEntity.PhoneNumber = customerToUpdate.PhoneNumber;            
            if (!string.IsNullOrWhiteSpace(customerToUpdate.IdentityDocumentNumber)) originalEntity.IdentityDocumentNumber = customerToUpdate.IdentityDocumentNumber;
            if (!string.IsNullOrWhiteSpace(customerToUpdate.Address)) originalEntity.Address = customerToUpdate.Address;
            if (!string.IsNullOrWhiteSpace(customerToUpdate.Email)) originalEntity.Email = customerToUpdate.Email;
            if (customerToUpdate.MaximumCreditAuthorized != default) originalEntity.MaximumCreditAuthorized = customerToUpdate.MaximumCreditAuthorized;
            if (customerToUpdate.AccountBalance != default) originalEntity.AccountBalance = customerToUpdate.AccountBalance;
            if (!string.IsNullOrWhiteSpace(customerToUpdate.Comment)) originalEntity.Comment = customerToUpdate.Comment;
            originalEntity.LastModificationDate = customerToUpdate.LastModificationDate;
            originalEntity.LastModificationUser = customerToUpdate.LastModificationUser;

            dbSet.Update(originalEntity);
        }
    }

}
