using System.ComponentModel.DataAnnotations;
using Kremis.Utility.Helpers;

namespace Kremis.Domain.Assemblers
{
    public class LocalityDto : BaseDto<int>
    {
        [Required(ErrorMessage = "Le nom est obligatoire;")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "La ville est obligatoire;")]
        public int? CityId { get; set; }
        public CityDto City { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Name.ToTitleCase() ?? ""}";
            }
        }
    }
}
