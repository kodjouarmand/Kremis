using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class LandTitleDocument : BaseEntity<int>
    {
        [Required]
        public string DocumentUrl { get; set; }

        [Required]
        public int LandTitleId { get; set; }
        [ForeignKey("LandTitleId")]
        public LandTitle LandTitle { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentType DocumentType { get; set; }
    }
}
