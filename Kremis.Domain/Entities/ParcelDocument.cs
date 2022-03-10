using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class ParcelDocument : BaseEntity<int>
    {
        [Required]
        public string DocumentUrl { get; set; }

        [Required]
        public int ParcelId { get; set; }
        [ForeignKey("ParcelId")]
        public Parcel Parcel { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentType DocumentType { get; set; }
    }
}
