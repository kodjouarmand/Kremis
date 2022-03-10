using System.Collections.Generic;
using System.Linq;
using Kremis.Domain.Assemblers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Domain.ViewModels
{
    public class ParcelViewModel
    {
        public ParcelDto Parcel { get; set; }
        public IEnumerable<ParcelDocumentDto> ParcelDocuments { get; set; }
        public IEnumerable<SelectListItem> LandTitleList { get; set; }
        public IEnumerable<SelectListItem> LocalityList { get; set; }
        public bool ReturnToDetailView { get; set; }

        public readonly IEnumerable<SelectListItem> RoadTypeList = new List<SelectListItem>()
        {
           new SelectListItem(){Text = "Accidenté"},
           new SelectListItem(){Text = "Légèrement praticable"},
           new SelectListItem(){Text = "Praticable"}
        }.OrderBy(u => u.Text);

        public readonly IEnumerable<SelectListItem> LandTypeList = new List<SelectListItem>()
        {
           new SelectListItem(){Text = "Agricole"},
           new SelectListItem(){Text = "Habitation"},
           new SelectListItem(){Text = "Marécageux"},
           new SelectListItem(){Text = "Industrielle"}
        }.OrderBy(u => u.Text);

        public readonly IEnumerable<SelectListItem> AreaMarkingList = new List<SelectListItem>()
        {
           new SelectListItem(){Text = "Habitable"},
           new SelectListItem(){Text = "Zone verte"}
        }.OrderBy(u => u.Text);

        public readonly IEnumerable<SelectListItem> YesNoList = new List<SelectListItem>()
        {
           new SelectListItem(){Text = "Non"},
           new SelectListItem(){Text = "Oui"}
        }.OrderBy(u => u.Text);
    }
}
