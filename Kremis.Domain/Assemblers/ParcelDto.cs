using System.ComponentModel.DataAnnotations;
using Kremis.Utility.Helpers;

namespace Kremis.Domain.Assemblers
{
    public class ParcelDto : BaseDto<int>
    {
        [Required(ErrorMessage = "Le N° est obligatoire;")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Le prix unitaire est obligatoire;")]
        public double? UnitPrice { get; set; }

        [Required(ErrorMessage = "Le prix minimum est obligatoire;")]
        public double? MinimumUnitPrice { get; set; }

        [Required(ErrorMessage = "La superficie est obligatoire;")]
        public double? Surface { get; set; }
        public string BlocNumber { get; set; }
        public string LandType { get; set; }
        public string RoadType { get; set; }
        public string AreaMarking { get; set; }
        public string HasWater { get; set; }
        public string HasElectrilocality { get; set; }
        public string HasTechnicalFile { get; set; }
        public string HasImages { get; set; }
        public string HasVideos { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public int? LandTitleId { get; set; }
        public LandTitleDto LandTitle { get; set; }

        [Required(ErrorMessage = "La localité est obligatoire;")]
        public int LocalityId { get; set; }
        public LocalityDto Locality { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Number ?? ""}";
            }
        }

    }
}
