using AutoMapper;
using Kremis.Domain.Entities;
using Kremis.BusinessLogic.Exceptions;
using System;
using System.Text;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class LandTitleCommand : BaseCommand<LandTitleDto, LandTitle, int>, ILandTitleCommand
    {
        private readonly ILandTitleQuery _landTitleQuery;
        public LandTitleCommand(IUnitOfWork unitOfWork, IMapper mapper,
            ILandTitleQuery landTitleQuery) : base(unitOfWork, mapper)
        {
            _landTitleQuery = landTitleQuery;
        }

        protected override StringBuilder ValidateAdd(LandTitleDto landTitleDto)
        {
            StringBuilder validationErrors = new();

            if (!landTitleDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new LandTitleValidator().Validate(landTitleDto);
            validationErrors.Append(validationResult.ToString());

            if (_landTitleQuery.GetByNumber(landTitleDto.Number) != null)
            {
                validationErrors.Append("Un titre foncier avec ce numéro existe déjà;\n");
            }
            return validationErrors;
        }

        public override int Add(LandTitleDto landTitleDto)
        {
            var landTitle = BuildEntity(landTitleDto);
            _unitOfWork.LandTitle.Add(landTitle);
            _unitOfWork.Save();
            return landTitle.Id;
        }

        protected override StringBuilder ValidateUpdate(LandTitleDto landTitleDto)
        {
            StringBuilder validationErrors = new();

            if (landTitleDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new LandTitleValidator().Validate(landTitleDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(LandTitleDto landTitleDto)
        {
            var landTitle = BuildEntity(landTitleDto);
            _unitOfWork.LandTitle.Update(landTitle);
        }

        protected override StringBuilder ValidateDelete(LandTitleDto landTitleDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(landTitleDto);
            return validationErrors;
        }

        public override void Delete(int landTitleId)
        {
            var landTitleDto = _landTitleQuery.GetById(landTitleId);
            StringBuilder validationErrors = ValidateDelete(landTitleDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.LandTitle.Delete(landTitleId);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }
    }
}
