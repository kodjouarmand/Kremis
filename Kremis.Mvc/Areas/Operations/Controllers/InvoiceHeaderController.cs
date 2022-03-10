using Microsoft.AspNetCore.Mvc;
using Kremis.Infrastructure.Contracts;
using Kremis.BusinessLogic.Enums;
using Kremis.Domain.Assemblers;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.BusinessLogic.Commands.Contracts;
using Kremis.Mvc;
using System;
using Kremis.Domain.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Kremis.Utility.Helpers;
using Kremis.Utility.Enum;
using System.Text;
using Microsoft.Extensions.Options;
using Kremis.Utility.Options;
using static Kremis.Utility.Helpers.MvcHelper;
using Developpez.Dotnet;
using Kremis.Domain.Paging;

namespace Kremis.Areas.Operations.Controllers
{
    public class InvoiceHeaderController : BaseOperationsController
    {
        private readonly IInvoiceHeaderQuery _invoiceHeaderQuery;
        private readonly IInvoiceHeaderCommand _invoiceHeaderCommand;
        private readonly IInvoiceDetailQuery _invoiceDetailQuery;
        private readonly IInvoiceDetailCommand _invoiceDetailCommand;
        private readonly IParcelQuery _parcelQuery;
        private readonly ICustomerQuery _customerQuery;
        private readonly IBusinessPartnerQuery _businessPartnerQuery;
        private readonly CompanySettingsOptions _companyDefaultValueSettings;

        public InvoiceHeaderController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            IInvoiceHeaderQuery invoiceHeaderQuery, IInvoiceHeaderCommand invoiceHeaderCommand, IInvoiceDetailQuery invoiceDetailQuery,
            IInvoiceDetailCommand invoiceDetailCommand, IParcelQuery parcelQuery,
            ICustomerQuery customerQuery, IBusinessPartnerQuery businessPartnerQuery,
            IOptions<CompanySettingsOptions> companyDefaultValueOptions)
            : base(logger, applicationUserQuery)
        {
            _invoiceHeaderQuery = invoiceHeaderQuery;
            _invoiceHeaderCommand = invoiceHeaderCommand;
            _customerQuery = customerQuery;
            _businessPartnerQuery = businessPartnerQuery;
            _invoiceDetailQuery = invoiceDetailQuery;
            _invoiceDetailCommand = invoiceDetailCommand;
            _parcelQuery = parcelQuery;
            _companyDefaultValueSettings = companyDefaultValueOptions.Value;
        }

        #region InvoiceHeader

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var invoiceHeaderDtos = _invoiceHeaderQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(invoiceHeaderDtos);
        }

        public IActionResult Summary(int id, bool returnToViewCommission = false)
        {
            InvoiceHeaderViewModel invoiceHeaderViewModel = GetInvoiceHeaderViewModel(id, loadDetail: true,
                returnToViewCommission: returnToViewCommission);
            return View(invoiceHeaderViewModel);
        }

        public IActionResult AddOrEdit(int? id, bool returnToHeaderView = false)
        {
            InvoiceHeaderViewModel invoiceHeaderViewModel = GetInvoiceHeaderViewModel(id, returnToHeaderView: returnToHeaderView);
            return View(invoiceHeaderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(InvoiceHeaderViewModel invoiceHeaderViewModel)
        {
            InvoiceHeaderDto invoiceHeaderDto = invoiceHeaderViewModel.InvoiceHeader;
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(invoiceHeaderDto.CommissionType))
                    {
                        invoiceHeaderDto.CommissionType = EnumHelper.ToString(CommissionTypeEnum.None);
                        invoiceHeaderDto.CommissionUnitValue = 0;
                    }
                    _invoiceHeaderCommand.CurrentUser = CurrentUser.UserName;
                    if (invoiceHeaderDto.Id == 0)
                    {
                        invoiceHeaderDto.Id = _invoiceHeaderCommand.Add(invoiceHeaderDto);
                        InvoiceDetailDto invoiceDetailDto = invoiceHeaderViewModel.InvoiceDetail;
                        invoiceDetailDto.InvoiceHeaderId = invoiceHeaderDto.Id;
                        AddDetail(invoiceDetailDto);
                    }
                    else
                    {
                        _invoiceHeaderCommand.Update(invoiceHeaderDto);
                        _invoiceHeaderCommand.Save();
                    }
                    return RedirectToAction("Summary", new { invoiceHeaderDto.Id, returnToViewCommission = false });
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
                invoiceHeaderViewModel = GetInvoiceHeaderViewModel(invoiceHeaderDto.Id);
                return View(invoiceHeaderViewModel);
            }
        }

        public IActionResult Print(int id)
        {
            InvoiceHeaderViewModel invoiceHeaderViewModel = GetInvoiceHeaderViewModel(id, loadDetail: true);
            return View(invoiceHeaderViewModel);
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _invoiceHeaderCommand.DbAction = DataBaseActionEnum.Delete;
                _invoiceHeaderCommand.Delete(id);
                _invoiceHeaderCommand.Save();
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
            var invoiceHeaderViewModel = new InvoiceHeaderViewModel
            {
                InvoiceHeader = new InvoiceHeaderDto() { Id = id }
            };
            return View(invoiceHeaderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(InvoiceHeaderViewModel invoiceHeaderViewModel)
        {
            try
            {
                CheckActionAuthorization();
                int id = invoiceHeaderViewModel.InvoiceHeader.Id;
                _invoiceHeaderCommand.DbAction = DataBaseActionEnum.Cancel;
                _invoiceHeaderCommand.Cancel(id, invoiceHeaderViewModel.InvoiceHeader.CancelationReason);
                _invoiceHeaderCommand.Save();

                var invoiceHeaderDtos = _invoiceHeaderQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    isValid = true,
                    message = "Opération effectuée avec succès.",
                    html = MvcHelper.RenderRazorViewToString(this, "_List", invoiceHeaderDtos)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);

                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "Cancel", invoiceHeaderViewModel)
                });
            }
        }

        private InvoiceHeaderViewModel GetInvoiceHeaderViewModel(int? invoiceHeaderId = null, bool loadDetail = false,
            bool returnToViewCommission = false, bool returnToHeaderView = false)
        {
            InvoiceHeaderDto invoiceHeaderDto = new();
            List<InvoiceDetailDto> invoiceDetails = new();
            if (invoiceHeaderId != null && invoiceHeaderId != 0)
            {
                invoiceHeaderDto = _invoiceHeaderQuery.GetById(invoiceHeaderId.GetValueOrDefault());
                if (loadDetail)
                {
                    invoiceDetails = _invoiceDetailQuery.GetByInvoiceHeaderId(invoiceHeaderId.GetValueOrDefault()).ToList();
                }
            }
            else
            {
                invoiceHeaderDto.Date = DateTime.Today.Date;
                invoiceHeaderDto.PaymentDueDate = DateTime.Today.AddDays(_companyDefaultValueSettings.PaymentDelay.GetValueOrDefault()).Date;
                invoiceHeaderDto.ParcellingCosts = _companyDefaultValueSettings.ParcellingCosts.GetValueOrDefault();
                invoiceHeaderDto.TechnicalFileCosts = _companyDefaultValueSettings.TechnicalFileCosts.GetValueOrDefault();
                invoiceHeaderDto.BoundaryCosts = _companyDefaultValueSettings.BoundaryCosts.GetValueOrDefault();
            }

            IEnumerable<SelectListItem> customers = _customerQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.DisplayText,
                Value = i.Id.ToString()
            });

            IEnumerable<SelectListItem> businessPartners = _businessPartnerQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.DisplayText,
                Value = i.Id.ToString()
            });

            IEnumerable<SelectListItem> commissionTypes = new List<SelectListItem>()
            {
                new SelectListItem(){Text = EnumHelper.ToString(CommissionTypeEnum.None), Selected = true},
                new SelectListItem(){Text = EnumHelper.ToString(CommissionTypeEnum.Fixed)},
                new SelectListItem(){Text = EnumHelper.ToString(CommissionTypeEnum.Percentage)}
            };

            InvoiceDetailDto invoiceDetailDto = new();
            List<ParcelDto> parcelDtos = _parcelQuery.GetByStatus(EnumHelper.ToString(StatusEnum.Available)).ToList();
            InvoiceHeaderViewModel invoiceHeaderViewModel = new()
            {
                InvoiceHeader = invoiceHeaderDto,
                InvoiceDetail = invoiceDetailDto,
                InvoiceDetails = invoiceDetails,
                CustomerList = customers,
                BusinessPartnerList = businessPartners,
                CommissionTypeList = commissionTypes,
                ReturnToViewCommission = returnToViewCommission,
                ReturnToHeaderView = returnToHeaderView,
                ParcelList = parcelDtos.OrderBy(u => u.LandTitle.Number).Select(i => new SelectListItem
                {
                    Text = i.DisplayText,
                    Value = i.Id.ToString()
                })
            };
            return invoiceHeaderViewModel;
        }

        public IActionResult CommissionIndex(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            return View();
        }

        public void AddDetail(InvoiceDetailDto invoiceDetailDto)
        {
            _invoiceDetailCommand.CurrentUser = CurrentUser.UserName;
            if (invoiceDetailDto.IsNew())
            {
                _invoiceDetailCommand.Add(invoiceDetailDto);
            }
            else
            {
                _invoiceDetailCommand.Update(invoiceDetailDto);
            }
            _invoiceDetailCommand.Save();
        }


        #endregion InvoiceHeader

        #region InvoiceDetail

        [NoDirectAccess]
        public IActionResult AddOrEditDetail(int? id, int? invoiceHeaderId)
        {
            InvoiceDetailViewModel invoiceDetailViewModel = GetInvoiceDetailViewModel(id, invoiceHeaderId);
            return View(invoiceDetailViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditDetail(InvoiceDetailViewModel invoiceDetailViewModel)
        {
            InvoiceDetailDto invoiceDetailDto = invoiceDetailViewModel.InvoiceDetail;
            try
            {
                if (ModelState.IsValid)
                {
                    _invoiceDetailCommand.CurrentUser = CurrentUser.UserName;
                    if (invoiceDetailDto.IsNew())
                    {
                        _invoiceDetailCommand.Add(invoiceDetailDto);
                    }
                    else
                    {
                        _invoiceDetailCommand.Update(invoiceDetailDto);
                    }
                    _invoiceDetailCommand.Save();

                    InvoiceHeaderViewModel invoiceHeaderViewModel = new()
                    {
                        InvoiceHeader = _invoiceHeaderQuery.GetById(invoiceDetailViewModel.InvoiceDetail.InvoiceHeaderId),
                        InvoiceDetails = _invoiceDetailQuery.GetByInvoiceHeaderId(invoiceDetailViewModel.InvoiceDetail.InvoiceHeaderId)
                    };

                    return Json(new
                    {
                        isValid = true,
                        message = "Opération effectuée avec succès.",
                        html = MvcHelper.RenderRazorViewToString(this, "_Header", invoiceHeaderViewModel)
                    });
                }
                else
                {
                    throw new Exception(MvcHelper.GetErrorMessages(ModelState));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);

                invoiceDetailViewModel = GetInvoiceDetailViewModel(invoiceDetailDto.Id, invoiceDetailDto.InvoiceHeaderId);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEditDetail", invoiceDetailViewModel)
                });
            }
        }

        public IActionResult DeleteInvoiceDetail(int id)
        {
            InvoiceDetailDto invoiceDetailDto = _invoiceDetailQuery.GetById(id);
            if (invoiceDetailDto == null)
            {
                return Json(new { success = false, message = "L'enregistrement a supprimer n'a pas été trouvé." });
            }
            _invoiceDetailCommand.DbAction = DataBaseActionEnum.Delete;
            StringBuilder validationErrors = new();
            _invoiceDetailCommand.Delete(id);
            if (validationErrors.Length != 0)
            {
                return Json(new { success = false, message = validationErrors.ToString() });
            }
            _invoiceDetailCommand.Save();

            return Json(new { success = true, message = "Opération effectuée avec succès" });
        }

        private InvoiceDetailViewModel GetInvoiceDetailViewModel(int? id = null, int? invoiceHeaderId = null)
        {
            InvoiceDetailDto invoiceDetailDto = new();
            InvoiceHeaderDto invoiceHeaderDto = new();
            if (id != null && id != 0)
            {
                invoiceDetailDto = _invoiceDetailQuery.GetById(id.GetValueOrDefault());
            }
            else if (invoiceHeaderId != null && invoiceHeaderId != 0)
            {
                invoiceDetailDto = new InvoiceDetailDto
                {
                    InvoiceHeaderId = invoiceHeaderId.GetValueOrDefault(),
                    InvoiceHeader = _invoiceHeaderQuery.GetById(invoiceHeaderId.GetValueOrDefault())
                };
            }

            List<ParcelDto> parcelDtos = _parcelQuery.GetByStatus(EnumHelper.ToString(StatusEnum.Available)).ToList();
            if (!invoiceDetailDto.IsNew())
            {
                ParcelDto parcelDto = _parcelQuery.GetById(invoiceDetailDto.ParcelId.GetValueOrDefault());
                parcelDtos.Add(parcelDto); ;
            }

            InvoiceDetailViewModel invoiceDetailViewModel = new()
            {
                InvoiceDetail = invoiceDetailDto,
                InvoiceHeader = invoiceHeaderDto,
                ParcelList = parcelDtos.OrderBy(u => u.LandTitle.Number).Select(i => new SelectListItem
                {
                    Text = i.DisplayText,
                    Value = i.Id.ToString()
                })
            };

            return invoiceDetailViewModel;
        }

        [HttpGet]
        public IActionResult GetParcel(int parcelId)
        {
            ParcelDto parcelDto = _parcelQuery.GetById(parcelId);
            if (parcelDto == null)
            {
                parcelDto = new ParcelDto();
            }

            return Json(parcelDto);
        }

        #endregion InvoiceDetail
    }

}