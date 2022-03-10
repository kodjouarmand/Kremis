using AutoMapper;
using Kremis.Domain.Entities;
using Kremis.BusinessLogic.Exceptions;
using System;
using System.Text;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class LocalityCommand : BaseCommand<LocalityDto, Locality, int>, ILocalityCommand
    {
        private readonly ILocalityQuery _localityQuery;
        public LocalityCommand(IUnitOfWork unitOfWork, IMapper mapper,
            ILocalityQuery localityQuery) : base(unitOfWork, mapper)
        {
            _localityQuery = localityQuery;
        }

        protected override StringBuilder ValidateAdd(LocalityDto localityDto)
        {
            StringBuilder validationErrors = new();

            if (!localityDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new LocalityValidator().Validate(localityDto);
            validationErrors.Append(validationResult.ToString());

            if (_localityQuery.GetByCityId(localityDto.CityId.GetValueOrDefault()).Any(u =>u.Name == localityDto.Name))
            {
                validationErrors.Append("Une localité avec ce nom existe déjà pour cette ville;\n");
            }
            return validationErrors;
        }

        public override int Add(LocalityDto localityDto)
        {
            var locality = BuildEntity(localityDto);
            _unitOfWork.Locality.Add(locality);
            _unitOfWork.Save();
            return locality.Id;
        }

        protected override StringBuilder ValidateUpdate(LocalityDto localityDto)
        {
            StringBuilder validationErrors = new();

            if (localityDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new LocalityValidator().Validate(localityDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(LocalityDto localityDto)
        {
            var locality = BuildEntity(localityDto);
            _unitOfWork.Locality.Update(locality);
        }

        protected override StringBuilder ValidateDelete(LocalityDto localityDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(localityDto);
            return validationErrors;
        }

        public override void Delete(int localityId)
        {
            var localityDto = _localityQuery.GetById(localityId);
            StringBuilder validationErrors = ValidateDelete(localityDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.Locality.Delete(localityId);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }
    }
}
