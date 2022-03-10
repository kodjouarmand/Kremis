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
    public class InvoicePaymentCommand : BaseCommand<InvoicePaymentDto, InvoicePayment, int>, IInvoicePaymentCommand
    {
        private readonly IInvoicePaymentQuery _invoicePaymentQuery;
        private readonly IInvoiceHeaderCommand _invoiceHeaderCommand;
        public InvoicePaymentCommand(IUnitOfWork unitOfWork, IMapper mapper,
            IInvoicePaymentQuery invoicePaymentQuery, IInvoiceHeaderCommand invoiceHeaderCommand) :
            base(unitOfWork, mapper)
        {
            _invoicePaymentQuery = invoicePaymentQuery;
            _invoiceHeaderCommand = invoiceHeaderCommand;
        }

        protected override StringBuilder ValidateAdd(InvoicePaymentDto invoicePaymentDto)
        {
            StringBuilder validationErrors = new();

            if (!invoicePaymentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez ajouter existe déjà;\n");
                return validationErrors;
            }

            var validationResult = new InvoicePaymentValidator(_unitOfWork, invoicePaymentDto).Validate(invoicePaymentDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override int Add(InvoicePaymentDto invoicePaymentDto)
        {
            UpdateInvoiceHeaderAdvancedAmount(invoicePaymentDto);
            var invoicePayment = BuildEntity(invoicePaymentDto);
            _unitOfWork.InvoicePayment.Add(invoicePayment);
            _unitOfWork.Save();
            return invoicePayment.Id;
        }

        protected override StringBuilder ValidateUpdate(InvoicePaymentDto invoicePaymentDto)
        {
            StringBuilder validationErrors = new();

            if (invoicePaymentDto.IsNew())
            {
                validationErrors.Append("L'enregistrement que vous souhaitez mettre à jour n'existe pas;");
                return validationErrors;
            }
            var validationResult = new InvoicePaymentValidator(_unitOfWork, invoicePaymentDto).Validate(invoicePaymentDto);
            validationErrors.Append(validationResult.ToString());

            return validationErrors;
        }

        public override void Update(InvoicePaymentDto invoicePaymentDto)
        {
            UpdateInvoiceHeaderAdvancedAmount(invoicePaymentDto);
            var invoicePayment = BuildEntity(invoicePaymentDto);
            _unitOfWork.InvoicePayment.Update(invoicePayment);
        }

        protected override StringBuilder ValidateDelete(InvoicePaymentDto invoicePaymentDto = null)
        {
            StringBuilder validationErrors = base.ValidateDelete(invoicePaymentDto);
            return validationErrors;
        }

        public override void Delete(int invoicePaymentId)
        {
            var invoicePaymentDto = _invoicePaymentQuery.GetById(invoicePaymentId);
            StringBuilder validationErrors = ValidateDelete(invoicePaymentDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }

            UpdateInvoiceHeaderAdvancedAmount(invoicePaymentDto);
            _unitOfWork.InvoicePayment.Delete(invoicePaymentId);
        }

        protected override StringBuilder ValidateCancel(InvoicePaymentDto invoicePaymentDto = null)
        {
            StringBuilder validationErrors = base.ValidateCancel(invoicePaymentDto);
            if (invoicePaymentDto!= null && string.IsNullOrWhiteSpace(invoicePaymentDto.CancelationReason))
            {
                validationErrors.Append($"Le motif de l'annulation est obligatoire;");
            }
            return validationErrors;
        }

        public override void Cancel(int invoicePaymentId, string cancelationReason = null)
        {
            var invoicePaymentDto = _invoicePaymentQuery.GetById(invoicePaymentId);
            StringBuilder validationErrors = ValidateCancel(invoicePaymentDto);
            if (validationErrors.Length != 0)
            {
                throw new BllValidationException(validationErrors.ToString());
            }
            UpdateInvoiceHeaderAdvancedAmount(invoicePaymentDto);
            _unitOfWork.InvoicePayment.Cancel(invoicePaymentId, cancelationReason);
        }

        public override void Save()
        {
            _unitOfWork.Save();
        }


        private void UpdateInvoiceHeaderAdvancedAmount(InvoicePaymentDto invoicePaymentDto)
        {
            var invoiceHeader = _unitOfWork.InvoiceHeader.GetById(invoicePaymentDto.InvoiceHeaderId.GetValueOrDefault());
            if (this.DbAction == DataBaseActionEnum.Save)
            {
                if (!invoicePaymentDto.IsNew())
                {
                    var originalInvoicePayment = _invoicePaymentQuery.GetById(invoicePaymentDto.Id);
                    invoiceHeader.AdvancedAmount -= originalInvoicePayment.AmountPaid.GetValueOrDefault();
                }

                invoiceHeader.AdvancedAmount += invoicePaymentDto.AmountPaid.GetValueOrDefault();
            }
            else
            {
                invoiceHeader.AdvancedAmount -= invoicePaymentDto.AmountPaid.GetValueOrDefault();
            }

            _invoiceHeaderCommand.UpdateAmounts(ref invoiceHeader);
            if (invoiceHeader.CommissionRemainingToPay < 0)
            {
                throw new BllValidationException("Le montant payé doit être inférieur au montant restant àa payer;");
            }
            _invoiceHeaderCommand.UpdateStatus(ref invoiceHeader);

            _unitOfWork.InvoiceHeader.Update(invoiceHeader);
        }
    }
}
