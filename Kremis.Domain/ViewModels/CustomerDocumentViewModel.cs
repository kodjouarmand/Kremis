using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class CustomerDocumentViewModel
    {
        public CustomerDocumentDto CustomerDocument { get; set; }
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        public IEnumerable<SelectListItem> DocumentTypeList { get; set; }
        public bool ReturnToIndexView { get; set; }
    }
}
