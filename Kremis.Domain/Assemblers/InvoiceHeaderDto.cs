using System;
using System.ComponentModel.DataAnnotations;
using Kremis.Utility.Enum;
using Kremis.Utility.Helpers;

namespace Kremis.Domain.Assemblers
{
    public class InvoiceHeaderDto : BaseDto<int>
    {
        public string Number { get { return Id.ToString().PadLeft(5, '0'); } }

        [Required(ErrorMessage = "La date est obligatoire;")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PaymentDueDate { get; set; }

        [Required(ErrorMessage = "Les frais de morcellement sont obligatoires;")]
        public double? ParcellingCosts { get; set; }

        [Required(ErrorMessage = "Les frais de dossier technique sont obligatoires;")]
        public double? TechnicalFileCosts { get; set; }

        [Required(ErrorMessage = "Les frais de bornage sont obligatoires;")]
        public double? BoundaryCosts { get; set; }

        public double? TotalSaleAmount { get; set; }
        public double? NetAmountToPay { get; set; }
        public double? AdvancedAmount { get; set; }
        public double? RemainingAmountToPay { get; set; }
        public string Status { get; set; }
        public string CommissionType { get; set; }
        public double? CommissionUnitValue { get; set; }
        public double? CommissionToPay { get; set; }
        public double? CommissionPaid { get; set; }
        public double? CommissionRemainingToPay { get; set; }
        public string CommissionStatus { get; set; }
        public bool IsCanceled { get; set; }
        public string CancelationReason { get; set; }
        public string Comment { get; set; }

        [Required(ErrorMessage = "Le client est obligatoire")]
        public int? CustomerId { get; set; }
        public CustomerDto Customer { get; set; }

        public int? BusinessPartnerId { get; set; }
        public BusinessPartnerDto BusinessPartner { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Number ?? ""}";
            }
        }

        public bool CanBeDeletedOrCanceled
        {
            get
            {
                if (Status != EnumHelper.ToString(StatusEnum.Unpaid))
                {
                    return false;
                }
                else if (CommissionStatus != EnumHelper.ToString(StatusEnum.Unpaid))
                {
                    return false;
                }
                else if (TotalSaleAmount != 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
