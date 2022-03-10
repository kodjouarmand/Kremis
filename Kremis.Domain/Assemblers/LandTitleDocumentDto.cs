using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Kremis.Domain.Assemblers
{
    public class LandTitleDocumentDto : BaseDto<int>
    {
        public string DocumentUrl { get; set; }

        [Required(ErrorMessage = "Le titre foncier est obligatoire;")]
        public int? LandTitleId { get; set; }
        [ForeignKey("LandTitleId")]
        public LandTitleDto LandTitle { get; set; }

        [Required(ErrorMessage = "Le type de document est obligatoire;")]
        public int? DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentTypeDto DocumentType { get; set; }
        public IFormFile Document { get; set; }
    }
}
