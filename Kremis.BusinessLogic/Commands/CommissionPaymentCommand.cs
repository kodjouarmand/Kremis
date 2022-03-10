using AutoMapper;
using Kremis.Domain.Entities;
using System.Text;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.DataAccess.Repositories.Contracts;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.BusinessLogic.Exceptions;

namespace Kremis.BusinessLogic.Commands.Contracts
{
    public class CommissionPaymentCommand : BaseCommand<CommissionPaymentDto, CommissionPayment, int>, ICommissionPaymentCommand
    {
        private readonly ICommissionPaymentQuery _commissionPaymentQuery;
        private readonly IInvoiceHeaderCommand _invoiceHeaderCommand;
        public CommissionPaymentCommand(IUnitOfWork unitOfWork, IMapper mapper,
            ICommissionPaymentQuery commissionPaymentQuery, IInvoiceHeaderCommand invoiceHeaderCommand) :
            base(unitOfWork, mapper)
        {
            _commissionPaymentQuery = commissionPaymentQuery;
            _invoiceHeaderCommand = invoiceHeaderCommand;
        }

        protected override StringBuilder ValidateAdd(CommissionPaymentDto commissionPaymentDto)
        {
            StringBuilder validationErrors = new();

            if (!commissionPaymentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new CommissionPaymentValidator(_unitOfWork, commissionPaymentDto).Validate(commissionPaymentDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override int Add(CommissionPaymentDto commissionPaymentDto)
        {
            UpdateInvoiceHeaderCommissionPaid(commissionPaymentDto);
            var commissionPayment = BuildEntity(commissionPaymentDto);
            _unitOfWork.CommissionPayment.Add(commissionPayment);
            _unitOfWork.Save();
            return commissionPayment.Id;
        }

        protected override StringBuilder ValidateUpdate(CommissionPaymentDto commissionPaymentDto)
        {
            StringBuilder validationErrors = new();

            if (commissionPaymentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas.");
                return validationErrors;
            }
            var validationResult = new CommissionPaymentValidator(_unitOfWork, commissionPaymentDto).Validate(commissionPaymentDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(CommissionPaymentDto commissionPaymentDto)
        {
            UpdateInvoiceHeaderCommissionPaid(commissionPaymentDto);
            var commissionPayment = BuildEntity(commissionPaymentDto);
            _unitOfWork.CommissionPayment.Update(commissionPayment);
        }

        protected override StringBuilder ValidateDelete(CommissionPaymentDto commissionPaymentDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(commissionPaymentDto);
            return validationErrors;
        }

        public override void Delete(int commissionPaymentId)
        {
            var commissionPaymentDto = _commissionPaymentQuery.GetById(commissionPaymentId);
            StringBuilder validationErrors = ValidateDelete(commissionPaymentDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }

            UpdateInvoiceHeaderCommissionPaid(commissionPaymentDto);
            _unitOfWork.CommissionPayment.Delete(commissionPaymentId);
        }

        protected override StringBuilder ValidateCancel(CommissionPaymentDto commissionPaymentDto = null)
        {
            StringBuilder validationErrors = base.ValidateCancel(commissionPaymentDto);
            if (string.IsNullOrWhiteSpace(commissionPaymentDto.CancelationReason))
            {
                validationErrors.Append($"Le motif de l'annulation est obligatoire;");
            }
            return validationErrors;
        }

        public override void Cancel(int commissionPaymentId, string cancelationReason = null)
        {
            var commissionPaymentDto = _commissionPaymentQuery.GetById(commissionPaymentId);
            StringBuilder validationErrors = ValidateCancel(commissionPaymentDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            UpdateInvoiceHeaderCommissionPaid(commissionPaymentDto);
            _unitOfWork.CommissionPayment.Cancel(commissionPaymentId, cancelationReason);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }


        private void UpdateInvoiceHeaderCommissionPaid(CommissionPaymentDto commissionPaymentDto)
        {
            var invoiceHeader = _unitOfWork.InvoiceHeader.GetById(commissionPaymentDto.InvoiceHeaderId.GetValueOrDefault());
            if (this.DbAction == DataBaseActionEnum.Save)
            {
                if (!commissionPaymentDto.IsNew())
                {
                    var originalCommissionPayment = _commissionPaymentQuery.GetById(commissionPaymentDto.Id);
                    invoiceHeader.CommissionPaid -= originalCommissionPayment.AmountPaid.GetValueOrDefault();
                }

                invoiceHeader.CommissionPaid += commissionPaymentDto.AmountPaid.GetValueOrDefault();
            }
            else
            {
                invoiceHeader.CommissionPaid -= commissionPaymentDto.AmountPaid.GetValueOrDefault();
            }

            invoiceHeader.CommissionRemainingToPay = invoiceHeader.CommissionToPay - invoiceHeader.CommissionPaid;
            if (invoiceHeader.CommissionRemainingToPay < 0)
            {
                throw new BllValidationException("Le montant payé doit être inférieur au montant restant àa payer;");
            }
            _invoiceHeaderCommand.UpdateCommissionStatus(ref invoiceHeader);

            _unitOfWork.InvoiceHeader.Update(invoiceHeader);
        }
    }
}
