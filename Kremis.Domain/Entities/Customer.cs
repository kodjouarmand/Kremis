using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class Customer : BaseEntity<int>
    {
        [NotMapped]
        public string Reference { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public double AccountBalance { get; set; }
        public double? MaximumCreditAuthorized { get; set; }
        public string Comment { get; set; }

        [Required]
        public string IdentityDocumentNumber { get; set; } 
        [Required]
        public int IdentityDocumentTypeId { get; set; }
        [ForeignKey("IdentityDocumentTypeId")]
        public IdentityDocumentType IdentityDocumentType { get; set; }
    }
}
