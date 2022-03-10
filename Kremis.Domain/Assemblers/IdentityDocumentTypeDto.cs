using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kremis.Utility.Helpers;

namespace Kremis.Domain.Assemblers
{
    public class IdentityDocumentTypeDto : BaseDto<int>
    {
        [Required(ErrorMessage ="*")]
        public string Name { get; set; }
        public string DisplayText
        {
            get
            {
                return Name.ToTitleCase();
            }
        }
    }
}
