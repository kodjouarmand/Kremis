using System.ComponentModel.DataAnnotations;
using Kremis.Utility.Helpers;

namespace Kremis.Domain.Assemblers
{
    public class CustomerDto : BaseDto<int>
    {
        public string Reference { get { return Id.ToString().PadLeft(5, '0'); } }

        [Required(ErrorMessage = "Le nom est obligatoire;")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le N° de téléphone est obligatoire;")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "L'adresse email est invalide;")]
        public string Email { get; set; }

        public double? AccountBalance { get; set; }

        public double? MaximumCreditAuthorized { get; set; }
        public string Comment { get; set; }

        [Required(ErrorMessage = "Le N° de la pièce d'identité est obligatoire;")]
        public string IdentityDocumentNumber { get; set; }

        [Required(ErrorMessage = "Le type de document d'identité est obligatoire;")]
        public int? IdentityDocumentTypeId { get; set; }
        public IdentityDocumentTypeDto IdentityDocumentType { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Name.ToUpper() ?? ""}";
            }
        }
    }
}
