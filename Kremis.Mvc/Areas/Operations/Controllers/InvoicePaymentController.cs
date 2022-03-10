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
using Kremis.Domain.Paging;

namespace Kremis.Areas.Operations.Controllers
{
    public class InvoicePaymentController : BaseOperationsController
    {
        private readonly IInvoicePaymentQuery _invoicePaymentQuery;
        private readonly IInvoicePaymentCommand _invoicePaymentCommand;
        private readonly IInvoiceHeaderQuery _invoiceHeaderQuery;
        private readonly IPaymentModeQuery _paymentModeQuery;

        public InvoicePaymentController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            IInvoicePaymentQuery invoicePaymentQuery, IInvoicePaymentCommand invoicePaymentCommand,
            IInvoiceHeaderQuery invoiceHeaderQuery, IPaymentModeQuery paymentModeQuery)
            : base(logger, applicationUserQuery)
        {
            _invoicePaymentQuery = invoicePaymentQuery;
            _invoicePaymentCommand = invoicePaymentCommand;
            _invoiceHeaderQuery = invoiceHeaderQuery;
            _paymentModeQuery = paymentModeQuery;
        }

        #region InvoicePayment

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var invoicePaymentDtos = _invoicePaymentQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(invoicePaymentDtos);
        }

        public IActionResult Summary(int id)
        {
            InvoicePaymentViewModel invoicePaymentViewModel = GetInvoicePaymentViewModel(id, true);
            return View(invoicePaymentViewModel);
        }

        public IActionResult AddOrEdit(int? id, bool returnToDetailView = false)
        {
            InvoicePaymentViewModel invoicePaymentViewModel = GetInvoicePaymentViewModel(id, returnToDetailView: returnToDetailView);
            return View(invoicePaymentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(InvoicePaymentViewModel invoicePaymentViewModel)
        {
            InvoicePaymentDto invoicePaymentDto = invoicePaymentViewModel.InvoicePayment;
            try
            {
                if (ModelState.IsValid)
                {
                    _invoicePaymentCommand.CurrentUser = CurrentUser.UserName;
                    if (invoicePaymentDto.Id == 0)
                    {
                        invoicePaymentDto.Id = _invoicePaymentCommand.Add(invoicePaymentDto);
                    }
                    else
                    {
                        _invoicePaymentCommand.Update(invoicePaymentDto);
                        _invoicePaymentCommand.Save();                       
                    }
                    return RedirectToAction("Summary", new { invoicePaymentDto.Id});
                }
                else
                {
                    throw new Exception(MvcHelper.GetErrorMessages(ModelState));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                TempData[ConstantHelper.TEMP_DATA_ERROR] = ex.Message.Replace("-","\n");
                invoicePaymentViewModel = GetInvoicePaymentViewModel(invoicePaymentDto.Id);
                return View(invoicePaymentViewModel);
            }
        }

        public IActionResult Print(int id)
        {
            InvoicePaymentViewModel invoiceHeaderViewModel = GetInvoicePaymentViewModel(id);
            return View(invoiceHeaderViewModel);
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _invoicePaymentCommand.DbAction = DataBaseActionEnum.Delete;
                _invoicePaymentCommand.Delete(id);
                _invoicePaymentCommand.Save();
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
            var invoicePaymentViewModel = new InvoicePaymentViewModel
            {
                InvoicePayment = new InvoicePaymentDto() { Id = id }
            };
            return View(invoicePaymentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(InvoicePaymentViewModel invoicePaymentViewModel)
        {
            try
            {
                CheckActionAuthorization();
                int id = invoicePaymentViewModel.InvoicePayment.Id;
                _invoicePaymentCommand.DbAction = DataBaseActionEnum.Cancel;
                _invoicePaymentCommand.Cancel(id, invoicePaymentViewModel.InvoicePayment.CancelationReason);
                _invoicePaymentCommand.Save();

                var invoicePaymentDtos = _invoicePaymentQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    isValid = true,
                    message = "Opération effectuée avec succès.",
                    html = MvcHelper.RenderRazorViewToString(this, "_List", invoicePaymentDtos)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);

                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "Cancel", invoicePaymentViewModel)
                });
            }
        }

        private InvoicePaymentViewModel GetInvoicePaymentViewModel(int? invoicePaymentId = null,
            bool returnToDetailView = false)
        {
            InvoicePaymentDto invoicePayment = new();
            if (invoicePaymentId != null && invoicePaymentId != 0)
            {
                invoicePayment = _invoicePaymentQuery.GetById(invoicePaymentId.GetValueOrDefault());
            }
            else
            {
                invoicePayment.Date = DateTime.Today.Date;
            }

            IEnumerable<SelectListItem> invoiceHeaders = _invoiceHeaderQuery.GetAll()
                .Where(u => u.Status != EnumHelper.ToString(StatusEnum.Paid))
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

            InvoicePaymentViewModel invoicePaymentViewModel = new()
            {
                InvoicePayment = invoicePayment,
                InvoiceHeaderList = invoiceHeaders,
                PaymentModeList = paymentModes,
                ReturnToDetailView = returnToDetailView
            };
            return invoicePaymentViewModel;
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