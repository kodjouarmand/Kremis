using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kremis.Utility.Helpers;

namespace Kremis.Domain.Assemblers
{
    public class PaymentModeDto : BaseDto<int>
    {
        [Required(ErrorMessage ="Le nom est obligatoire;")]
        public string Name { get; set; }
        public string DisplayText
        {
            get
            {
                return $"{Name.ToTitleCase() ?? ""}";
            }
        }
    }
}
