using FluentValidation;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.Domain.Assemblers;
using Kremis.Utility.Helpers;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class ParcelValidator : AbstractValidator<ParcelDto>
    {
        public ParcelValidator(IUnitOfWork unitOfWork, int? landTitleId)
        {
            RuleFor(u => u.Number).NotNull().NotEmpty()
                .WithMessage("Le numéro d'identificationest obligatoire; ");

            RuleFor(u => u.LocalityId).NotNull().NotEqual(0).NotEmpty()
                .WithMessage("La localité est obligatoire;\n");

            if (landTitleId != null)
            {
                var landTitle = unitOfWork.LandTitle.GetById(landTitleId.GetValueOrDefault());

                RuleFor(u => u.LocalityId).Equal(landTitle.LocalityId)
                .WithMessage("La localité de la parcel doit être la même que celle du titre foncier;\n");
            }

            RuleFor(u => u.Surface).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("La superficie doit être supérieure à 0;\n");

            RuleFor(u => u.UnitPrice).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("Le prix du m² doit être supérieur à 0;\n");

            RuleFor(u => u.MinimumUnitPrice).NotNull().NotEmpty().NotEqual(0)
                .WithMessage("Le prix minimum du m² doit être supérieure à 0;\n");

            RuleFor(u => u.MinimumUnitPrice).LessThanOrEqualTo(u => u.UnitPrice)
                .WithMessage("Le prix minimum du m² doit être inférieur ou égale au prix du m²;\n");

            //RuleFor(u => u.LandType).NotNull().NotEmpty()
            //    .WithMessage("Le type de terrain est obligatoire;\n");

            //RuleFor(u => u.RoadType).NotNull().NotEmpty()
            //    .WithMessage("Le type de route est obligatoire;\n");

            //RuleFor(u => u.AreaMarking).NotNull().NotEmpty()
            //    .WithMessage("Le répérage de zone est obligatoire;\n");

            //RuleFor(u => u.HasTechnicalFile).NotNull().NotEmpty()
            //    .WithMessage("Veuillez indiquer si le dossier technique est disponible;\n");

            //RuleFor(u => u.HasWater).NotNull().NotEmpty()
            //    .WithMessage("Veuillez indiquer si l'eau est disponible;\n");

            //RuleFor(u => u.HasElectrilocality).NotNull().NotEmpty()
            //    .WithMessage("Veuillez indiquer si l'électricité est disponible;\n");

            //RuleFor(u => u.HasImages).NotNull().NotEmpty()
            //    .WithMessage("Veuillez indiquer si des images sont disponible;\n");

            //RuleFor(u => u.HasVideos).NotNull().NotEmpty()
            //    .WithMessage("Veuillez indiquer si des vidéos sont disponible;\n");
        }
    }
}
