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
    public class LocalityController : BaseAdminController
    {
        private readonly ILocalityQuery _localityQuery;
        private readonly ILocalityCommand _localityCommand;
        private readonly ICityQuery _cityQuery;

        public LocalityController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            ILocalityQuery localityQuery, ILocalityCommand localityCommand, ICityQuery cityQuery)
            : base(logger, applicationUserQuery)
        {
            _localityQuery = localityQuery;
            _localityCommand = localityCommand;
            _cityQuery = cityQuery;
        }

        #region Locality

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var localityDtos = _localityQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(localityDtos);
        }

        public IActionResult Summary(int id)
        {
            LocalityViewModel localityViewModel = GetLocalityViewModel(id);
            return View(localityViewModel);
        }

        [NoDirectAccess]
        public IActionResult AddOrEdit(int? id, bool returnToDetailView = false)
        {
            LocalityViewModel localityViewModel = GetLocalityViewModel(id, returnToDetailView: returnToDetailView);
            return View(localityViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(LocalityViewModel localityViewModel)
        {
            LocalityDto localityDto = localityViewModel.Locality;
            try
            {
                if (ModelState.IsValid)
                {
                    _localityCommand.CurrentUser = CurrentUser.UserName;
                    if (localityDto.Id == 0)
                    {
                        localityDto.Id = _localityCommand.Add(localityDto);
                    }
                    else
                    {
                        _localityCommand.Update(localityDto);
                    }
                    _localityCommand.Save();

                    string returnHtml;
                    if (localityViewModel.ReturnToDetailView)
                    {
                        localityViewModel = GetLocalityViewModel(localityDto.Id, 
                            returnToDetailView: localityViewModel.ReturnToDetailView);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_Detail", localityViewModel);
                    }
                    else
                    {
                        var localityDtos = _localityQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_List", localityDtos);
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

                localityViewModel = GetLocalityViewModel(localityDto.Id);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEdit", localityViewModel)
                });
            }
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _localityCommand.DbAction = DataBaseActionEnum.Delete;
                _localityCommand.Delete(id);
                _localityCommand.Save();
                return Json(new { success = true, message = "Opération effectuée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Json(new { success = false, message = $"Echec lors de l'exécution de l'opération : {ex.Message}" });
            }
        }

        public LocalityViewModel GetLocalityViewModel(int? localityId = null, 
            bool returnToDetailView = false)
        {
            LocalityDto locality = new();
            if (localityId != null && localityId != 0)
            {
                locality = _localityQuery.GetById(localityId.GetValueOrDefault());
            }

            IEnumerable<SelectListItem> cities = _cityQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.DisplayText,
                Value = i.Id.ToString()
            });

            LocalityViewModel localityViewModel = new()
            {
                Locality = locality,
                CityList = cities,
                ReturnToDetailView = returnToDetailView
            };

            return localityViewModel;
        }

        #endregion Locality
    }

}