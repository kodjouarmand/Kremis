using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class LandTitle : BaseEntity<int>
    {
        [Required]
        public string Number { get; set; }

        [Required]
        public string Owner { get; set; }

        [Required]
        public double Surface { get; set; }
        public string Description { get; set; }

        [Required]
        public int LocalityId { get; set; }
        [ForeignKey("LocalityId")]
        public Locality Locality { get; set; }
    }
}
