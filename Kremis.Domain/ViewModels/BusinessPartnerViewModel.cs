using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class BusinessPartnerViewModel
    {
        public BusinessPartnerDto BusinessPartner { get; set; }
        public bool ReturnToDetailView { get; set; }    
        public int? InvoiceHeaderId { get; set; }
    }
}
