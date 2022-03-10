using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Kremis.Domain.Assemblers
{
    public class CustomerDocumentDto : BaseDto<int>
    {
        public string DocumentUrl { get; set; }

        [Required(ErrorMessage ="Le client est obligatoire;")]
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public CustomerDto Customer { get; set; }

        [Required(ErrorMessage ="Le type de document est obligatoire;")]
        public int? DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentTypeDto DocumentType { get; set; }

        public IFormFile Document { get; set; }
    }
}
