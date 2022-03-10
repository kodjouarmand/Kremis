using AutoMapper;
using Kremis.Domain.Entities;
using System.Text;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Utility.Helpers;
using Kremis.Utility.Enum;
using System.Linq;
using Kremis.BusinessLogic.Exceptions;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class InvoiceDetailCommand : BaseCommand<InvoiceDetailDto, InvoiceDetail, int>, IInvoiceDetailCommand
    {
        private readonly IInvoiceDetailQuery _invoiceDetailQuery;
        private readonly IInvoiceHeaderCommand _invoiceHeaderCommand;
        public InvoiceDetailCommand(IUnitOfWork unitOfWork, IMapper mapper,
            IInvoiceDetailQuery invoiceDetailQuery, IInvoiceHeaderCommand invoiceHeaderCommand) : base(unitOfWork, mapper)
        {
            _invoiceDetailQuery = invoiceDetailQuery;
            _invoiceHeaderCommand = invoiceHeaderCommand;
        }

        protected override StringBuilder ValidateAdd(InvoiceDetailDto invoiceDetailDto)
        {
            StringBuilder validationErrors = new();

            if (!invoiceDetailDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new InvoiceDetailValidator(_unitOfWork, invoiceDetailDto).Validate(invoiceDetailDto);
            validationErrors.Append(validationResult.ToString());

            if (_invoiceDetailQuery.GetByParcelId(invoiceDetailDto.ParcelId.GetValueOrDefault()) != null)
            {
                validationErrors.Append("Ce lot n'est pas disponible;\n");
                return validationErrors;
            }
            return validationErrors;
        }

        public override int Add(InvoiceDetailDto invoiceDetailDto)
        {
            UpdateInvoiceHeader(invoiceDetailDto);
            UpdateParcelStatus(invoiceDetailDto);
            var invoiceDetail = BuildEntity(invoiceDetailDto);
            return _unitOfWork.InvoiceDetail.Add(invoiceDetail);
        }

        protected override StringBuilder ValidateUpdate(InvoiceDetailDto invoiceDetailDto)
        {
            StringBuilder validationErrors = new();

            if (invoiceDetailDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new InvoiceDetailValidator(_unitOfWork, invoiceDetailDto).Validate(invoiceDetailDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(InvoiceDetailDto invoiceDetailDto)
        {
            UpdateInvoiceHeader(invoiceDetailDto);
            UpdateParcelStatus(invoiceDetailDto);
            var invoiceDetail = BuildEntity(invoiceDetailDto);
            _unitOfWork.InvoiceDetail.Update(invoiceDetail);
        }

        protected override StringBuilder ValidateDelete(InvoiceDetailDto invoiceDetailDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(invoiceDetailDto);
            return validationErrors;
        }

        public override void Delete(int invoiceDetailId)
        {
            var invoiceDetailDto = _invoiceDetailQuery.GetById(invoiceDetailId);
            StringBuilder validationErrors = ValidateDelete(invoiceDetailDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            UpdateInvoiceHeader(invoiceDetailDto);
            UpdateParcelStatus(invoiceDetailDto);
            _unitOfWork.InvoiceDetail.Delete(invoiceDetailId);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }

        private void UpdateInvoiceHeader(InvoiceDetailDto invoiceDetailDto)
        {
            var invoiceHeader = _unitOfWork.InvoiceHeader.GetById(invoiceDetailDto.InvoiceHeaderId);
            CalculateInvoiceHeaderTotalSaleAmount(ref invoiceHeader, invoiceDetailDto);
            _invoiceHeaderCommand.UpdateAmounts(ref invoiceHeader);
            _invoiceHeaderCommand.UpdateStatus(ref invoiceHeader);
            UpdateInvoiceHeaderCommission(ref invoiceHeader, invoiceDetailDto);
            _invoiceHeaderCommand.UpdateCommissionStatus(ref invoiceHeader);
            _unitOfWork.InvoiceHeader.Update(invoiceHeader);
        }

        private void CalculateInvoiceHeaderTotalSaleAmount(ref InvoiceHeader invoiceHeader, InvoiceDetailDto invoiceDetailDto)
        {
            if (this.DbAction == DataBaseActionEnum.Save)
            {
                if (!invoiceDetailDto.IsNew())
                {
                    var originalInvoiceDetail = _invoiceDetailQuery.GetById(invoiceDetailDto.Id);
                    invoiceHeader.TotalSaleAmount -= originalInvoiceDetail.Total.GetValueOrDefault();
                }

                invoiceHeader.TotalSaleAmount += invoiceDetailDto.Total.GetValueOrDefault();
            }
            else
            {
                invoiceHeader.TotalSaleAmount -= invoiceDetailDto.Total.GetValueOrDefault();
            }
        }

        private void UpdateInvoiceHeaderCommission(ref InvoiceHeader invoiceHeader, InvoiceDetailDto invoiceDetailDto)
        {
            if (invoiceHeader.CommissionType == EnumHelper.ToString(CommissionTypeEnum.Percentage))
            {
                invoiceHeader.CommissionToPay = invoiceHeader.TotalSaleAmount * invoiceHeader.CommissionUnitValue / 100;
            }
            else if (invoiceHeader.CommissionType == EnumHelper.ToString(CommissionTypeEnum.Fixed))
            {                
                var commission = invoiceDetailDto.Surface.GetValueOrDefault() * invoiceHeader.CommissionUnitValue;

                if (this.DbAction == DataBaseActionEnum.Save)
                {
                    if (!invoiceDetailDto.IsNew())
                    {
                        var originalInvoiceDetail = _invoiceDetailQuery.GetById(invoiceDetailDto.Id);
                        invoiceHeader.CommissionToPay -= 
                            originalInvoiceDetail.Surface.GetValueOrDefault() * invoiceHeader.CommissionUnitValue;
                    }

                    invoiceHeader.CommissionToPay += commission;
                }
                else
                {
                    invoiceHeader.CommissionToPay -= commission;
                }
            }
            invoiceHeader.CommissionRemainingToPay = invoiceHeader.CommissionToPay - invoiceHeader.CommissionPaid;
        }

        private void UpdateParcelStatus(InvoiceDetailDto invoiceDetailDto)
        {
            var parcel = _unitOfWork.Parcel.GetById(invoiceDetailDto.ParcelId.GetValueOrDefault());
            if (this.DbAction == DataBaseActionEnum.Save)
            {
                if (!invoiceDetailDto.IsNew())
                {
                    var originalInvoiceDetail = _unitOfWork.InvoiceDetail.GetById(invoiceDetailDto.Id);
                    if (originalInvoiceDetail.ParcelId != invoiceDetailDto.ParcelId)
                    {
                        var previousParcel = _unitOfWork.Parcel.GetById(originalInvoiceDetail.ParcelId);
                        previousParcel.Status = EnumHelper.ToString(StatusEnum.Available);
                        _unitOfWork.Parcel.Update(previousParcel);
                    }
                }
                parcel.Status = EnumHelper.ToString(StatusEnum.Unvailable);
            }
            else
            {
                parcel.Status = EnumHelper.ToString(StatusEnum.Available);
            }
            _unitOfWork.Parcel.Update(parcel);
        }
    }
}
