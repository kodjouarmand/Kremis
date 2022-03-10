using FluentValidation;
using Kremis.Domain.Assemblers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class LandTitleValidator : AbstractValidator<LandTitleDto>
    {
        public LandTitleValidator()
        {
            RuleFor(u => u.Number).NotNull().NotEmpty()
                .WithMessage("Le numéro du titre foncier est obligatoire;\n");

            RuleFor(u => u.Owner).NotNull().NotEmpty()
                .WithMessage("Le propriétaire du titre foncier est obligatoire;\n");

            RuleFor(u => u.LocalityId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("La localité est obligatoire;\n");

            RuleFor(u => u.Surface).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("La superficie doit être supérieure à 0;\n");
        }
    }
}
