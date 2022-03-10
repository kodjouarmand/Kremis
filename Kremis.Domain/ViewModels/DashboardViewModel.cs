using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Kremis.Domain.Paging;

namespace Kremis.Domain.ViewModels
{
    public class DashboardViewModel
    {
        public PagedResult<ParcelDto> AvailableParcels { get; set; }
        public PagedResult<InvoiceHeaderDto> UnpaidInvoices { get; set; }
        public PagedResult<InvoiceHeaderDto> OverdueInvoices { get; set; }
        public PagedResult<CustomerDto> DebtorCustomers  { get; set; }
    }
}
