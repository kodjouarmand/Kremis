using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class LandTitleDocumentViewModel
    {
        public LandTitleDocumentDto LandTitleDocument { get; set; }
        public IEnumerable<SelectListItem> DocumentTypeList { get; set; }
    }
}
