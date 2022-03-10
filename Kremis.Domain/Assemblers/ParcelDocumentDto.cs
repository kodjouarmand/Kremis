using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Kremis.Domain.Assemblers
{
    public class ParcelDocumentDto : BaseDto<int>
    {
        public string DocumentUrl { get; set; }

        [Required(ErrorMessage = "Le lot est obligatoire;")]
        public int? ParcelId { get; set; }
        [ForeignKey("ParcelId")]
        public ParcelDto Parcel { get; set; }

        [Required(ErrorMessage = "Le type de document est obligatoire;")]
        public int? DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentTypeDto DocumentType { get; set; }

        public IFormFile Document { get; set; }
    }
}
