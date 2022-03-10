using System;
using FluentValidation;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.Domain.Assemblers;
using Kremis.Utility.Helpers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class CommissionPaymentValidator : AbstractValidator<CommissionPaymentDto>
    {
        public CommissionPaymentValidator(IUnitOfWork _unitOfWork, CommissionPaymentDto commissionPaymentDto)
        {
            RuleFor(u => u.InvoiceHeaderId).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("La facture obligatoire;\n");

            RuleFor(u => u.PaymentModeId).NotNull().NotEmpty().NotEqual(0)
               .WithMessage("Le mode de paiement est obligatoire;\n");

            RuleFor(u => u.Date).NotNull().NotEmpty().NotEqual(default(DateTime))
                .WithMessage("La date de paiement obligatoire;\n");

            var paymentMode = _unitOfWork.PaymentMode.GetById(commissionPaymentDto.PaymentModeId.GetValueOrDefault());
            if (paymentMode.Name != ConstantHelper.PAYMENT_MODE_CASH)
            {
                RuleFor(u => u.TransactionNumber).NotNull().NotEmpty()
                  .WithMessage("Le numéro de la transaction est obligatoire;\n");
            }

            RuleFor(u => u.AmountPaid).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("Le montant payé est obligatoire;\n");

            if (commissionPaymentDto.IsCanceled)
            {
                RuleFor(u => u.CancelationReason).NotNull().NotEmpty()
                   .WithMessage("La raison de l'annulation est obligatoire;\n");
            }
        }
    }
}
