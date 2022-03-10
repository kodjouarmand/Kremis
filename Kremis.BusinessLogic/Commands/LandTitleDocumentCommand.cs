using AutoMapper;
using Kremis.Domain.Entities;
using Kremis.BusinessLogic.Exceptions;
using System;
using System.Text;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Utility.Enum;
using Kremis.Utility.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class LandTitleDocumentCommand : BaseCommand<LandTitleDocumentDto, LandTitleDocument, int>, ILandTitleDocumentCommand
    {
        private readonly ILandTitleDocumentQuery _landTitleDocumentQuery;
        private readonly IWebHostEnvironment _hostEnvironment;

        public LandTitleDocumentCommand(IUnitOfWork unitOfWork, IMapper mapper,
            ILandTitleDocumentQuery landTitleDocumentQuery, IWebHostEnvironment hostEnvironment) : base(unitOfWork, mapper)
        {
            _landTitleDocumentQuery = landTitleDocumentQuery;
            _hostEnvironment = hostEnvironment;
        }

        protected override StringBuilder ValidateAdd(LandTitleDocumentDto landTitleDocumentDto)
        {
            StringBuilder validationErrors = new();

            if (!landTitleDocumentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new LandTitleDocumentValidator().Validate(landTitleDocumentDto);
            validationErrors.Append(validationResult.ToString());

            if (_landTitleDocumentQuery.GetByDocumentUrl(landTitleDocumentDto.DocumentUrl) != null)
            {
                validationErrors.Append("Un document avec ce nom existe déjà;\n");
                return validationErrors;
            }
            return validationErrors;
        }

        public override int Add(LandTitleDocumentDto landTitleDocumentDto)
        {
            var landTitleDocument = BuildEntity(landTitleDocumentDto);
            var landTitleDocumentId = _unitOfWork.LandTitleDocument.Add(landTitleDocument);
            FileHelper.CreateFile(landTitleDocumentDto.Document, DocumentOwnerEnum.LandTitle, _hostEnvironment.WebRootPath);
            return landTitleDocumentId;
        }

        protected override StringBuilder ValidateUpdate(LandTitleDocumentDto landTitleDocumentDto)
        {
            StringBuilder validationErrors = new();

            if (landTitleDocumentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new LandTitleDocumentValidator().Validate(landTitleDocumentDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(LandTitleDocumentDto landTitleDocumentDto)
        {
            LandTitleDocumentDto originalLandTitleDocumentDto = _landTitleDocumentQuery.GetById(landTitleDocumentDto.Id);
            if (landTitleDocumentDto.Document == null)
            {
                landTitleDocumentDto.DocumentUrl = originalLandTitleDocumentDto.DocumentUrl;
            }

            var landTitleDocument = BuildEntity(landTitleDocumentDto);
            _unitOfWork.LandTitleDocument.Update(landTitleDocument);
            FileHelper.CreateFile(landTitleDocumentDto.Document, DocumentOwnerEnum.LandTitle, _hostEnvironment.WebRootPath,
                originalLandTitleDocumentDto.DocumentUrl);
        }

        protected override StringBuilder ValidateDelete(LandTitleDocumentDto landTitleDocumentDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(landTitleDocumentDto);
            return validationErrors;
        }

        public override void Delete(int landTitleDocumentId)
        {
            var landTitleDocumentDto = _landTitleDocumentQuery.GetById(landTitleDocumentId);
            StringBuilder validationErrors = ValidateDelete(landTitleDocumentDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.LandTitleDocument.Delete(landTitleDocumentId);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }
    }
}
