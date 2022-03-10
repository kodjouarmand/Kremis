using Kremis.Domain.Assemblers;
using System.Collections.Generic;
using Kremis.BusinessLogic.Enums;
using System.Text;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public interface IBaseCommand<TDto, TEntityKey> where TDto : BaseDto<TEntityKey>
    {
        DataBaseActionEnum DbAction { get; set; }
        string CurrentUser { get; set; }

        TEntityKey Add(TDto dto);
        void Update(TDto dto);
        void Delete(TEntityKey dtoId);
        void Cancel(TEntityKey dtoId, string cancelationReason = null);
        void Save();
    }
}