using FluentValidation;
using Kremis.Domain.Assemblers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class CustomerDocumentValidator : AbstractValidator<CustomerDocumentDto>
    {
        public CustomerDocumentValidator()
        {
            RuleFor(u => u.CustomerId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le client est obligatoire;\n");

            RuleFor(u => u.DocumentTypeId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("Le type du document est obligatoire;\n");
            
            RuleFor(u => u.DocumentUrl).NotNull().NotEmpty()
                .WithMessage("Le document scanné est obligatoire;\n");
        }
    }
}
