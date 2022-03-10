using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class InvoiceDetailViewModel
    {
        public InvoiceDetailDto InvoiceDetail { get; set; }
        public InvoiceHeaderDto InvoiceHeader { get; set; }
        public IEnumerable<SelectListItem> ParcelList { get; set; }
    }
}
