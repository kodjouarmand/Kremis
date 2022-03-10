using Microsoft.AspNetCore.Mvc;
using Kremis.Infrastructure.Contracts;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.BusinessLogic.Commands.Contracts;
using Kremis.Mvc;
using Kremis.Domain.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Kremis.Utility.Helpers;
using System;
using Kremis.Utility.Enum;
using static Kremis.Utility.Helpers.MvcHelper;
using Kremis.Domain.Paging;

namespace Kremis.Areas.Operations.Controllers
{
    public class CommissionPaymentController : BaseOperationsController
    {
        private readonly ICommissionPaymentQuery _commissionPaymentQuery;
        private readonly ICommissionPaymentCommand _commissionPaymentCommand;
        private readonly IInvoiceHeaderQuery _invoiceHeaderQuery;
        private readonly IPaymentModeQuery _paymentModeQuery;

        public CommissionPaymentController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            ICommissionPaymentQuery commissionPaymentQuery, ICommissionPaymentCommand commissionPaymentCommand,
            IInvoiceHeaderQuery invoiceHeaderQuery, IPaymentModeQuery paymentModeQuery)
            : base(logger, applicationUserQuery)
        {
            _commissionPaymentQuery = commissionPaymentQuery;
            _commissionPaymentCommand = commissionPaymentCommand;
            _invoiceHeaderQuery = invoiceHeaderQuery;
            _paymentModeQuery = paymentModeQuery;
        }

        #region CommissionPayment

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var commissionPaymentDtos = _commissionPaymentQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(commissionPaymentDtos);
        }

        public IActionResult Summary(int id)
        {
            CommissionPaymentViewModel commissionPaymentViewModel = GetCommissionPaymentViewModel(id, true);
            return View(commissionPaymentViewModel);
        }

        public IActionResult AddOrEdit(int? id, bool returnToDetailView = false)
        {
            CommissionPaymentViewModel commissionPaymentViewModel = GetCommissionPaymentViewModel(id, returnToDetailView: returnToDetailView);
            return View(commissionPaymentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(CommissionPaymentViewModel commissionPaymentViewModel)
        {
            CommissionPaymentDto commissionPaymentDto = commissionPaymentViewModel.CommissionPayment;
            try
            {
                if (ModelState.IsValid)
                {
                    _commissionPaymentCommand.CurrentUser = CurrentUser.UserName;
                    if (commissionPaymentDto.Id == 0)
                    {
                        commissionPaymentDto.Id = _commissionPaymentCommand.Add(commissionPaymentDto);
                    }
                    else
                    {
                        _commissionPaymentCommand.Update(commissionPaymentDto);
                        _commissionPaymentCommand.Save();
                    }
                    return RedirectToAction("Summary", new { commissionPaymentDto.Id });
                }
                else
                {
                    throw new Exception(MvcHelper.GetErrorMessages(ModelState));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                TempData[ConstantHelper.TEMP_DATA_ERROR] = ex.Message.Replace("-", "\n");
                commissionPaymentViewModel = GetCommissionPaymentViewModel(commissionPaymentDto.Id);
                return View(commissionPaymentViewModel);
            }
        }

        public IActionResult Print(int id)
        {
            CommissionPaymentViewModel invoiceHeaderViewModel = GetCommissionPaymentViewModel(id);
            return View(invoiceHeaderViewModel);
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _commissionPaymentCommand.DbAction = DataBaseActionEnum.Delete;
                _commissionPaymentCommand.Delete(id);
                _commissionPaymentCommand.Save();
                return Json(new { success = true, message = "Opération effectuée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Json(new { success = false, message = $"Echec lors de l'exécution de l'opération : {ex.Message}" });
            }
        }

        public IActionResult Cancel(int id)
        {
            var commissionPaymentViewModel = new CommissionPaymentViewModel
            {
                CommissionPayment = new CommissionPaymentDto() { Id = id }
            };
            return View(commissionPaymentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(CommissionPaymentViewModel commissionPaymentViewModel)
        {
            try
            {
                CheckActionAuthorization();
                int id = commissionPaymentViewModel.CommissionPayment.Id;
                _commissionPaymentCommand.DbAction = DataBaseActionEnum.Cancel;
                _commissionPaymentCommand.Cancel(id, commissionPaymentViewModel.CommissionPayment.CancelationReason);
                _commissionPaymentCommand.Save();

                var commissionPaymentDtos = _commissionPaymentQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    isValid = true,
                    message = "Opération effectuée avec succès.",
                    html = MvcHelper.RenderRazorViewToString(this, "_List", commissionPaymentDtos)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);

                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "Cancel", commissionPaymentViewModel)
                });
            }
        }

        private CommissionPaymentViewModel GetCommissionPaymentViewModel(int? commissionPaymentId = null,
            bool returnToDetailView = false)
        {
            CommissionPaymentDto commissionPayment = new();
            if (commissionPaymentId != null && commissionPaymentId != 0)
            {
                commissionPayment = _commissionPaymentQuery.GetById(commissionPaymentId.GetValueOrDefault());
            }
            else
            {
                commissionPayment.Date = DateTime.Today.Date;
            }

            IEnumerable<SelectListItem> invoiceHeaders = _invoiceHeaderQuery.GetAll().Where(u => u.CommissionRemainingToPay != 0)
                .Where(u => u.CommissionStatus != EnumHelper.ToString(StatusEnum.Paid))
                .Select(i => new SelectListItem
                {
                    Text = i.DisplayText,
                    Value = i.Id.ToString()
                });

            IEnumerable<SelectListItem> paymentModes = _paymentModeQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.DisplayText,
                Value = i.Id.ToString()
            });

            CommissionPaymentViewModel commissionPaymentViewModel = new()
            {
                CommissionPayment = commissionPayment,
                InvoiceHeaderList = invoiceHeaders,
                PaymentModeList = paymentModes,
                ReturnToDetailView = returnToDetailView
            };
            return commissionPaymentViewModel;
        }

        [HttpGet]
        public IActionResult GetInvoiceHeader(int invoiceHeaderId)
        {
            InvoiceHeaderDto invoiceHeaderDto = _invoiceHeaderQuery.GetById(invoiceHeaderId);
            if (invoiceHeaderDto == null)
            {
                invoiceHeaderDto = new InvoiceHeaderDto();
            }
            return Json(invoiceHeaderDto);
        }

        #endregion

    }

}