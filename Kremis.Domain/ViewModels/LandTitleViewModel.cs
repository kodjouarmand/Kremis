using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class LandTitleViewModel
    {
        public LandTitleDto LandTitle { get; set; }
        public IEnumerable<LandTitleDocumentDto> LandTitleDocuments { get; set; }
        public IEnumerable<SelectListItem> LocalityList { get; set; }
        public bool ReturnToDetailView { get; set; }
    }
}
