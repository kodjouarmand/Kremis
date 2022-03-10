using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kremis.Domain.Entities
{
    public class InvoiceHeader : BaseEntity<int>
    {
        [NotMapped]
        public string Number { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public DateTime? PaymentDueDate { get; set; }

        [Required]
        public double ParcellingCosts  { get; set; }

        [Required]
        public double TechnicalFileCosts { get; set; }

        [Required]
        public double BoundaryCosts  { get; set; }

        [Required]
        public double TotalSaleAmount { get; set; }

        [Required]
        public double NetAmountToPay { get; set; }

        [Required]
        public double AdvancedAmount { get; set; }

        [Required]
        public double RemainingAmountToPay { get; set; }

        public string Status { get; set; }

        [Required]
        public string CommissionType { get; set; }

        [Required]
        public double CommissionUnitValue { get; set; }

        [Required]
        public double CommissionToPay { get; set; }

        [Required]
        public double CommissionPaid { get; set; }

        [Required]
        public double CommissionRemainingToPay { get; set; }
        
        public string CommissionStatus { get; set; }
        public string Comment { get; set; }

        [Required]
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public int? BusinessPartnerId { get; set; }
        [ForeignKey("BusinessPartnerId")]
        public BusinessPartner BusinessPartner { get; set; }
    }
}
