using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class InvoiceDetail : BaseEntity<int>
    {
        [Required]
        public double UnitPrice { get; set; }

        [Required]
        public double Surface { get; set; }

        public double Total { get; set; }

        [Required]
        public int InvoiceHeaderId { get; set; }
        [ForeignKey("InvoiceHeaderId")]
        public InvoiceHeader InvoiceHeader { get; set; }

        [Required]
        public int ParcelId { get; set; }
        [ForeignKey("ParcelId")]
        public Parcel Parcel { get; set; }
    }
}
