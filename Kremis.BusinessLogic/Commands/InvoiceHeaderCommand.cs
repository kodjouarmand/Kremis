using AutoMapper;
using Kremis.Domain.Entities;
using System.Text;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.Utility.Enum;
using Kremis.Utility.Helpers;
using Kremis.BusinessLogic.Queries.Contracts;
using System.Linq;
using System;
using Kremis.BusinessLogic.Exceptions;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class InvoiceHeaderCommand : BaseCommand<InvoiceHeaderDto, InvoiceHeader, int>, IInvoiceHeaderCommand
    {
        private const string deleteOrCancelErrorMessage = @"Un règlement a déjà été effectué ou
                    une commission a déjà été payée ou des lots sont liés à la facture;";
        private readonly IInvoiceHeaderQuery _invoiceHeaderQuery;
        public InvoiceHeaderCommand(IUnitOfWork unitOfWork, IMapper mapper,
            IInvoiceHeaderQuery invoiceHeaderQuery) : base(unitOfWork, mapper)
        {
            _invoiceHeaderQuery = invoiceHeaderQuery;
        }

        protected override StringBuilder ValidateAdd(InvoiceHeaderDto invoiceHeaderDto)
        {
            StringBuilder validationErrors = new();

            if (!invoiceHeaderDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new InvoiceHeaderValidator(invoiceHeaderDto).Validate(invoiceHeaderDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override int Add(InvoiceHeaderDto invoiceHeaderDto)
        {
            var invoiceHeader = BuildEntity(invoiceHeaderDto);
            UpdateAmounts(ref invoiceHeader);
            UpdateStatus(ref invoiceHeader);
            UpdateCommission(ref invoiceHeader);
            UpdateCommissionStatus(ref invoiceHeader);
            _unitOfWork.InvoiceHeader.Add(invoiceHeader);
            _unitOfWork.Save();
            return invoiceHeader.Id;
        }

        protected override StringBuilder ValidateUpdate(InvoiceHeaderDto invoiceHeaderDto)
        {
            StringBuilder validationErrors = new();

            if (invoiceHeaderDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new InvoiceHeaderValidator(invoiceHeaderDto).Validate(invoiceHeaderDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(InvoiceHeaderDto invoiceHeaderDto)
        {
            var invoiceHeader = BuildEntity(invoiceHeaderDto);
            UpdateAmounts(ref invoiceHeader);
            UpdateStatus(ref invoiceHeader);
            UpdateCommission(ref invoiceHeader);
            UpdateCommissionStatus(ref invoiceHeader);
            _unitOfWork.InvoiceHeader.Update(invoiceHeader);
        }

        protected override StringBuilder ValidateDelete(InvoiceHeaderDto invoiceHeaderDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(invoiceHeaderDto);
            if (invoiceHeaderDto != null && !invoiceHeaderDto.CanBeDeletedOrCanceled)
            {
                validationErrors.Append(deleteOrCancelErrorMessage);
            }

            return validationErrors;
        }

        public override void Delete(int invoiceHeaderId)
        {
            var invoiceHeaderDto = _invoiceHeaderQuery.GetById(invoiceHeaderId);
            StringBuilder validationErrors = ValidateDelete(invoiceHeaderDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.InvoiceHeader.Delete(invoiceHeaderId);
        }

        protected override StringBuilder ValidateCancel(InvoiceHeaderDto invoiceHeaderDto = null)
        {
            StringBuilder validationErrors = base.ValidateCancel(invoiceHeaderDto);
            if (invoiceHeaderDto != null)
            {
                if (string.IsNullOrWhiteSpace(invoiceHeaderDto.CancelationReason))
                {
                    validationErrors.Append($"Le motif de l'annulation est obligatoire;");
                }
                if (!invoiceHeaderDto.CanBeDeletedOrCanceled)
                {
                    validationErrors.Append(deleteOrCancelErrorMessage);
                }
            }
            return validationErrors;
        }

        public override void Cancel(int invoiceHeaderId, string cancelationReason = null)
        {
            var invoiceHeaderDto = _invoiceHeaderQuery.GetById(invoiceHeaderId);
            invoiceHeaderDto.CancelationReason = cancelationReason;
            StringBuilder validationErrors = ValidateCancel(invoiceHeaderDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            _unitOfWork.InvoiceHeader.Cancel(invoiceHeaderId, cancelationReason);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }

        public void UpdateAmounts(ref InvoiceHeader invoiceHeader)
        {
            invoiceHeader.NetAmountToPay = invoiceHeader.ParcellingCosts + invoiceHeader.TechnicalFileCosts +
                invoiceHeader.BoundaryCosts + invoiceHeader.TotalSaleAmount;
            invoiceHeader.RemainingAmountToPay = invoiceHeader.NetAmountToPay - invoiceHeader.AdvancedAmount;
        }

        private void UpdateCommission(ref InvoiceHeader invoiceHeader)
        {
            if (invoiceHeader.CommissionType == EnumHelper.ToString(CommissionTypeEnum.Percentage))
            {
                invoiceHeader.CommissionToPay = invoiceHeader.TotalSaleAmount * invoiceHeader.CommissionUnitValue / 100;
            }
            else if (invoiceHeader.CommissionType == EnumHelper.ToString(CommissionTypeEnum.Fixed))
            {
                var invoiceHeaderId = invoiceHeader.Id;
                var totalSurface = _unitOfWork.InvoiceDetail.GetAll(u => u.InvoiceHeaderId == invoiceHeaderId)
                    .Sum(u => u.Surface);
                invoiceHeader.CommissionToPay = totalSurface * invoiceHeader.CommissionUnitValue;
            }
            invoiceHeader.CommissionRemainingToPay = invoiceHeader.CommissionToPay - invoiceHeader.CommissionPaid;
        }

        public void UpdateStatus(ref InvoiceHeader invoiceHeader)
        {
            if (invoiceHeader.AdvancedAmount == 0)
            {
                invoiceHeader.Status = EnumHelper.ToString(StatusEnum.Unpaid);
            }
            else if (invoiceHeader.AdvancedAmount == invoiceHeader.NetAmountToPay)
            {
                invoiceHeader.Status = EnumHelper.ToString(StatusEnum.Paid);
            }
            else
            {
                invoiceHeader.Status = EnumHelper.ToString(StatusEnum.PatiallyPaid);
            }
        }

        public void UpdateCommissionStatus(ref InvoiceHeader invoiceHeader)
        {
            if (invoiceHeader.CommissionPaid == 0)
            {
                invoiceHeader.CommissionStatus = EnumHelper.ToString(StatusEnum.Unpaid);
            }
            else if (invoiceHeader.CommissionPaid == invoiceHeader.CommissionToPay)
            {
                invoiceHeader.CommissionStatus = EnumHelper.ToString(StatusEnum.Paid);
            }
            else
            {
                invoiceHeader.CommissionStatus = EnumHelper.ToString(StatusEnum.PatiallyPaid);
            }
        }
    }
}
