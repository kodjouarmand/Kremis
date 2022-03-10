using FluentValidation;
using Kremis.Domain.Assemblers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(u => u.Name).NotNull().NotEmpty()
                .WithMessage("Le nom du client est obligatoire;\n");

            RuleFor(u => u.IdentityDocumentTypeId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le type de pièce d'identité est obligatoire;\n");

            RuleFor(u => u.IdentityDocumentNumber).NotNull().NotEmpty()
                .WithMessage("Le numéro de la pièce d'identité est obligatoire;\n");

            RuleFor(u => u.PhoneNumber).NotNull().NotEmpty()
                .WithMessage("Le numéro de téléphone est obligatoire;\n");
        }
    }
}
