using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class BusinessPartner : BaseEntity<int>
    {
        [NotMapped]
        public string Reference { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public double AccountBalance { get; set; }
        public string Comment { get; set; }
    }
}
