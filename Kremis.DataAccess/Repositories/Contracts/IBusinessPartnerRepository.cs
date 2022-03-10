using System;
using Kremis.Domain.Entities;

namespace Kremis.DataAccess.Repositories.Contracts
{
    public interface IBusinessPartnerRepository : IBaseRepository<BusinessPartner, int>
    {
        public void Update(BusinessPartner businessPartner);        
    }
}
