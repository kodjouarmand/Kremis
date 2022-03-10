using AutoMapper;
using Kremis.Domain.Entities;
using Kremis.Domain.Assemblers;
using System;
using System.Collections.Generic;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;
using Kremis.BusinessLogic.Exceptions;

namespace Kremis.BusinessLogic.Queries
{
    public abstract class BaseQuery<TDto, TEntity, TEntityKey> 
        : IBaseQuery<TDto, TEntityKey> where TDto 
        : BaseDto<TEntityKey> where TEntity : BaseEntity<TEntityKey>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public BaseQuery(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public abstract TDto GetById(TEntityKey id);
        public abstract IEnumerable<TDto> GetAll();

        protected TDto MapEntityToDto(TEntity entity, bool returnDeletedRecord = false)
        {
            if (!returnDeletedRecord)
            {
                if (entity !=null && entity.IsCanceled)
                {
                    throw new BllValidationException("The record has been marked as deleted;");
                }
            }
            return _mapper.Map<TDto>(entity);
        }

        protected IEnumerable<TDto> MapEntitiesToDto(IEnumerable<TEntity> entities, bool returnDeletedRecords = false)
        {
            if (!returnDeletedRecords)
            {
                entities = entities.Where(u => u.IsCanceled == false);
            }
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }
    }
}
