using System;
using FluentValidation;
using Kremis.Domain.Assemblers;
using Kremis.Utility.Enum;
using Kremis.Utility.Helpers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class InvoiceHeaderValidator : AbstractValidator<InvoiceHeaderDto>
    {
        public InvoiceHeaderValidator(InvoiceHeaderDto invoiceHeaderDto)
        {
            RuleFor(u => u.CustomerId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le client est obligatoire;\n");

            RuleFor(u => u.Date).NotNull().NotEmpty().NotEqual(default(DateTime))
                .WithMessage("La date de la vente obligatoire;\n");

            RuleFor(u => u.PaymentDueDate).NotNull().NotEmpty().NotEqual(default(DateTime))
                .WithMessage("La date d'échéance est obligatoire;\n");

            RuleFor(u => u.Date).LessThanOrEqualTo(u => u.PaymentDueDate)
                .WithMessage("La date d'échéance doit être supérieure ou égale à la date de la vente;\n");

            RuleFor(u => u.ParcellingCosts).NotNull().NotEmpty()
                .WithMessage("Le montant des frais de morcellement est obligatoire;\n");

            RuleFor(u => u.TechnicalFileCosts).NotNull().NotEmpty()
                .WithMessage("Le montant des frais de dossier technique est obligatoire;\n");

            RuleFor(u => u.BoundaryCosts).NotNull().NotEmpty()
                .WithMessage("Le montant des frais de bornage est obligatoire;\n");

            if (invoiceHeaderDto.BusinessPartnerId != null && invoiceHeaderDto.BusinessPartnerId != 0)
            {
                RuleFor(u => u.CommissionType).NotNull().NotEmpty().NotEqual(EnumHelper.ToString(CommissionTypeEnum.None))
                    .WithMessage("Le type de commission est obligatoire lorsque qu'un apporteur d'affaire est sélectionné;\n");

                RuleFor(u => u.CommissionUnitValue).NotNull().NotEmpty().NotEqual(0)
                   .WithMessage("La valeur de la commission est obligatoire;\n");
            }
            else if (!string.IsNullOrWhiteSpace(invoiceHeaderDto.CommissionType)
                && (invoiceHeaderDto.CommissionType != EnumHelper.ToString(CommissionTypeEnum.None)))
            {
                RuleFor(u => u.BusinessPartnerId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("L'apporteur d'affaire est obligatoire lorsque qu'un type de commission est sélectionné;\n");

                RuleFor(u => u.CommissionUnitValue).NotNull().NotEmpty().NotEqual(0)
                   .WithMessage("La valeur de la commission est obligatoire;\n");
            }
            if (invoiceHeaderDto.IsCanceled)
            {               
                RuleFor(u => u.CancelationReason).NotNull().NotEmpty()
                   .WithMessage("La raison de l'annulation est obligatoire;\n");
            }
        }
    }
}
