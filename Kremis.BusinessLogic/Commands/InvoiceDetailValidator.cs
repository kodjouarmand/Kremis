using FluentValidation;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.Domain.Assemblers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class InvoiceDetailValidator : AbstractValidator<InvoiceDetailDto>
    {
        public InvoiceDetailValidator(IUnitOfWork _unitOfWork, InvoiceDetailDto invoiceDetailDto)
        {
            var parcel = _unitOfWork.Parcel.GetById(invoiceDetailDto.ParcelId.GetValueOrDefault());
            RuleFor(u => u.ParcelId).NotNull().NotEmpty()
                .WithMessage("Le lot est obligatoire;\n");

            RuleFor(u => u.InvoiceHeaderId).NotNull().NotEmpty()
                .WithMessage("La facture obligatoire;\n");

            RuleFor(u => u.Surface).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("La superficie doit être supérieure à 0;\n");

            RuleFor(u => u.UnitPrice).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("Le prix du m² doit être supérieur à 0;\n");

            RuleFor(u => u.UnitPrice).GreaterThanOrEqualTo(parcel.MinimumUnitPrice)
               .WithMessage("Le prix du m² doit être supérieur ou égal au prix minimum autorisé pour ce lot;\n");
        }
    }
}
