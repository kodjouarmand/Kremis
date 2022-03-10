using AutoMapper;
using Kremis.Domain.Entities;
using Kremis.Domain.Assemblers;
using System;
using System.Text;
using Kremis.BusinessLogic.Exceptions;
using Kremis.BusinessLogic.Enums;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Commands.Contracts;

namespace Kremis.BusinessLogic.Commands
{
    public abstract class BaseCommand<TDto, TEntity, TEntityKey> : IBaseCommand<TDto, TEntityKey> where TDto : BaseDto<TEntityKey> where TEntity : BaseEntity<TEntityKey>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        public DataBaseActionEnum DbAction { get; set; }
        public string CurrentUser { get; set; }

        public BaseCommand(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            DbAction = DataBaseActionEnum.Save;
        }

        public abstract TEntityKey Add(TDto dto);
        public abstract void Update(TDto dto);
        public abstract void Delete(TEntityKey dtoId);
        public virtual void Cancel(TEntityKey dtoId, string cancelationReason = null) { }
        public abstract void Save();

        protected TEntity BuildEntity(TDto dto)
        {
            StringBuilder validationErrors = new();

            if (string.IsNullOrWhiteSpace(CurrentUser))
            {
                validationErrors.Append("L'utilisateur qui effectue l'opération est requis.");
                throw new BllValidationException(validationErrors.ToString());
            }
            if (DbAction != DataBaseActionEnum.Save)
            {
                validationErrors.Append("DataBaseAction n'est pas mis à Save.");
                throw new BllValidationException(validationErrors.ToString());
            }
            if (dto.IsNew())
            {
                validationErrors = ValidateAdd(dto);
            }
            else
            {
                validationErrors = ValidateUpdate(dto);
            }

            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }

            TEntity entity = MapDtoToEntity(dto);
            return entity;
        }

        protected abstract StringBuilder ValidateAdd(TDto dto);

        protected abstract StringBuilder ValidateUpdate(TDto dto);

        protected virtual StringBuilder ValidateDelete(TDto dto)
        {
            StringBuilder validationErrors = new();
            if (DbAction != DataBaseActionEnum.Delete)
            {
                validationErrors.Append("Impossible de supprimer : DB Action not set to Delete;");
            }
            return validationErrors;
        }

        protected virtual StringBuilder ValidateCancel(TDto dto)
        {
            StringBuilder validationErrors = new();
            if (DbAction != DataBaseActionEnum.Cancel)
            {
                validationErrors.Append("Impossible de supprimer : DB Action not set to Cancel;");
            }
            return validationErrors;
        }

        protected TEntity MapDtoToEntity(TDto dto)
        {
            TEntity entity = _mapper.Map<TEntity>(dto);

            if (dto.IsNew())
            {
                entity.IsCanceled = false;
                entity.CreationDate = DateTime.Now;
                entity.CreationUser = CurrentUser;
            }
            else
            {
                entity.LastModificationDate = DateTime.Now;
                entity.LastModificationUser = CurrentUser;
            }

            return entity;
        }
    }
}
