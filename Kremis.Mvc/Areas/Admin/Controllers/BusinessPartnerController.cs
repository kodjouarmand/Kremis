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
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Kremis.Utility.Helpers;
using Kremis.Utility.Enum;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Http;
using static Kremis.Utility.Helpers.MvcHelper;
using System;
using Kremis.Domain.Paging;

namespace Kremis.Areas.Admin.Controllers
{
    public class BusinessPartnerController : BaseAdminController
    {
        private readonly IBusinessPartnerQuery _businessPartnerQuery;
        private readonly IBusinessPartnerCommand _businessPartnerCommand;

        public BusinessPartnerController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            IBusinessPartnerQuery businessPartnerQuery, IBusinessPartnerCommand businessPartnerCommand)
            : base(logger, applicationUserQuery)
        {
            _businessPartnerQuery = businessPartnerQuery;
            _businessPartnerCommand = businessPartnerCommand;
        }

        #region BusinessPartner

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var businessPartnerDtos = _businessPartnerQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(businessPartnerDtos);
        }

        public IActionResult Summary(int id)
        {
            BusinessPartnerViewModel businessPartnerViewModel = GetBusinessPartnerViewModel(id, true);
            return View(businessPartnerViewModel);
        }

        [NoDirectAccess]
        public IActionResult AddOrEdit(int? id, bool returnToDetailView = false)
        {
            BusinessPartnerViewModel businessPartnerViewModel = GetBusinessPartnerViewModel(id, returnToDetailView: returnToDetailView);
            return View(businessPartnerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(BusinessPartnerViewModel businessPartnerViewModel)
        {
            BusinessPartnerDto businessPartnerDto = businessPartnerViewModel.BusinessPartner;
            try
            {
                if (ModelState.IsValid)
                {
                    _businessPartnerCommand.CurrentUser = CurrentUser.UserName;
                    if (businessPartnerDto.Id == 0)
                    {
                        businessPartnerDto.Id = _businessPartnerCommand.Add(businessPartnerDto);
                    }
                    else
                    {
                        _businessPartnerCommand.Update(businessPartnerDto);
                    }
                    _businessPartnerCommand.Save();

                    string returnHtml;
                    if (businessPartnerViewModel.ReturnToDetailView)
                    {
                        businessPartnerViewModel = GetBusinessPartnerViewModel(businessPartnerDto.Id,
                            returnToDetailView: businessPartnerViewModel.ReturnToDetailView);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_Detail", businessPartnerViewModel);
                    }
                    else
                    {
                        var businessPartnerDtos = _businessPartnerQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_List", businessPartnerDtos);
                    }
                    return Json(new
                    {
                        isValid = true,
                        message = "Opération effectuée avec succès.",
                        html = returnHtml
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

                businessPartnerViewModel = GetBusinessPartnerViewModel(businessPartnerDto.Id);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEdit", businessPartnerViewModel)
                });
            }
        }
      
        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _businessPartnerCommand.DbAction = DataBaseActionEnum.Delete;
                _businessPartnerCommand.Delete(id);
                _businessPartnerCommand.Save();
                return Json(new { success = true, message = "Opération effectuée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Json(new { success = false, message = $"Echec lors de l'exécution de l'opération : {ex.Message}" });
            }
        }

        private BusinessPartnerViewModel GetBusinessPartnerViewModel(int? businessPartnerId = null,
            bool returnToDetailView = false)
        {
            BusinessPartnerDto businessPartner = new();
            if (businessPartnerId != null && businessPartnerId != 0)
            {
                businessPartner = _businessPartnerQuery.GetById(businessPartnerId.GetValueOrDefault());                
            }

            BusinessPartnerViewModel businessPartnerViewModel = new()
            {
                BusinessPartner = businessPartner,
                ReturnToDetailView = returnToDetailView
            };
            return businessPartnerViewModel;

        }

        #endregion BusinessPartner
    }

}