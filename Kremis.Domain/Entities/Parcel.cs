using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class Parcel : BaseEntity<int>
    {
        [Required]
        public string Number { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        [Required]
        public double MinimumUnitPrice { get; set; }

        [Required]
        public double Surface { get; set; }
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
        [ForeignKey("LandTitleId")]
        public LandTitle LandTitle { get; set; }

        [Required]
        public int LocalityId { get; set; }
        [ForeignKey("LocalityId")]
        public Locality Locality { get; set; }
    }
}
