using System;
using System.ComponentModel.DataAnnotations;
using Kremis.Utility.Helpers;

namespace Kremis.Domain.Assemblers
{
    public class BusinessPartnerDto : BaseDto<int>
    {
        public string Reference { get { return Id.ToString().PadLeft(5, '0'); } }

        [Required(ErrorMessage ="Le nom est obligatoire;")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le numéro de téléphone est obligatoire;")]
        public string PhoneNumber { get; set; }
        public double? AccountBalance { get; set; }
        public string Comment { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Name.ToUpper() ?? ""}";
            }
        }
    }
}
