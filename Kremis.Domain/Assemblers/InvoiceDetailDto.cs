using System.ComponentModel.DataAnnotations;

namespace Kremis.Domain.Assemblers
{
    public class InvoiceDetailDto : BaseDto<int>
    {
        [Required(ErrorMessage = "Le prix unitaire est obligatoire;")]

        public double? UnitPrice { get; set; }

        [Required(ErrorMessage = "La superficie est obligatoire;")]
        public double? Surface { get; set; }

        public double? Total { get { return UnitPrice * Surface; } }

        [Required(ErrorMessage = "La facture est obligatoire;")]
        public int InvoiceHeaderId { get; set; }
        public InvoiceHeaderDto InvoiceHeader { get; set; }

        [Required(ErrorMessage ="Le lot est obligatoire")]
        public int? ParcelId { get; set; }
        public ParcelDto Parcel { get; set; }
    }
}
