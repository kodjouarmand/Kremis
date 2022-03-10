using System;
using System.ComponentModel.DataAnnotations;

namespace Kremis.Domain.Assemblers
{
    public class InvoicePaymentDto : BaseDto<int>
    {
        public string Number { get { return Id.ToString().PadLeft(5, '0'); } }

        [Required(ErrorMessage = "Le montant du règlement est obligatoire;")]
        public double? AmountPaid { get; set; }

        [Required(ErrorMessage = "La date du règlement est obligatoire;")]
        [DataType(DataType.Date)]
        public DateTime? Date{ get; set; }

        public string TransactionNumber { get; set; }

        public bool IsCanceled { get; set; }
        public string CancelationReason { get; set; }

        [Required(ErrorMessage = "La facture est obligatoire;")]
        public int? InvoiceHeaderId { get; set; }
        public InvoiceHeaderDto InvoiceHeader { get; set; }

        [Required(ErrorMessage = "Le mode de règlement est obligatoire;")]
        public int? PaymentModeId { get; set; }
        public PaymentModeDto PaymentMode { get; set; }
    }
}
