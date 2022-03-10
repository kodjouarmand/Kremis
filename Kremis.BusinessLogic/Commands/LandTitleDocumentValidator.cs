using FluentValidation;
using Kremis.Domain.Assemblers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class LandTitleDocumentValidator : AbstractValidator<LandTitleDocumentDto>
    {
        public LandTitleDocumentValidator()
        {
            RuleFor(u => u.DocumentUrl).NotNull().NotEmpty()
                .WithMessage("Le document scanné est obligatoire;\n");

            RuleFor(u => u.LandTitleId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le titre foncier est obligatoire;\n");

            RuleFor(u => u.DocumentTypeId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le type du document est obligatoire;\n");
        }
    }
}
