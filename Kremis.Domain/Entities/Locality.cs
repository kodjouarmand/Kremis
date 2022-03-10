using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class Locality : BaseEntity<int>
    {

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }
    }
}
