using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class ParcelDocumentViewModel
    {
        public ParcelDocumentDto ParcelDocument { get; set; }
        public IEnumerable<SelectListItem> ParcelList { get; set; }
        public IEnumerable<SelectListItem> DocumentTypeList { get; set; }
        public bool ReturnToIndexView { get; set; }
    }
}
