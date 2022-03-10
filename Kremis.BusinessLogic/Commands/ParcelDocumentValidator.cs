using FluentValidation;
using Kremis.Domain.Assemblers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class ParcelDocumentValidator : AbstractValidator<ParcelDocumentDto>
    {
        public ParcelDocumentValidator()
        {
            RuleFor(u => u.ParcelId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le lot est obligatoire;\n");

            RuleFor(u => u.DocumentTypeId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le type du document est obligatoire;\n");
            
            RuleFor(u => u.DocumentUrl).NotNull().NotEmpty()
                .WithMessage("Le document scanné est obligatoire;\n");
        }
    }
}
