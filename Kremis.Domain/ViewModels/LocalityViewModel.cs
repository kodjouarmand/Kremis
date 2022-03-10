using System.Collections.Generic;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class LocalityViewModel
    {
        public LocalityDto Locality { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }
        public bool ReturnToDetailView { get; set; }
    }
}
