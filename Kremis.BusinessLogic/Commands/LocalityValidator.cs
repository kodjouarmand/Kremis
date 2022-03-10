using FluentValidation;
using Kremis.Domain.Assemblers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class LocalityValidator : AbstractValidator<LocalityDto>
    {
        public LocalityValidator()
        {
            RuleFor(u => u.CityId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("La ville est obligatoire;\n");

            RuleFor(u => u.Name).NotNull().NotEmpty()
                .WithMessage("Le nom est obligatoire;\n");
        }
    }
}
