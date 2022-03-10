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
    public class LandTitleController : BaseAdminController
    {
        private readonly ILandTitleQuery _landTitleQuery;
        private readonly ILandTitleCommand _landTitleCommand;
        private readonly ILandTitleDocumentQuery _landTitleDocumentQuery;
        private readonly ILandTitleDocumentCommand _landTitleDocumentCommand;
        private readonly ILocalityQuery _localityQuery;
        private readonly IDocumentTypeQuery _documentTypeQuery;
        private readonly IWebHostEnvironment _hostEnvironment;

        public LandTitleController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            ILandTitleQuery landTitleQuery, ILandTitleCommand landTitleCommand,
            ILandTitleDocumentQuery landTitleDocumentQuery, ILandTitleDocumentCommand landTitleDocumentCommand,
             ILocalityQuery localityQuery, IDocumentTypeQuery documentTypeQuery,
             IWebHostEnvironment hostEnvironment)
            : base(logger, applicationUserQuery)
        {
            _landTitleQuery = landTitleQuery;
            _landTitleCommand = landTitleCommand;
            _localityQuery = localityQuery;
            _landTitleDocumentQuery = landTitleDocumentQuery;
            _landTitleDocumentCommand = landTitleDocumentCommand;
            _documentTypeQuery = documentTypeQuery;
            _hostEnvironment = hostEnvironment;
        }

        #region LandTitle

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var landTitleDtos = _landTitleQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(landTitleDtos);
        }

        public IActionResult Summary(int id)
        {
            LandTitleViewModel landTitleViewModel = GetLandTitleViewModel(id, true);
            return View(landTitleViewModel);
        }

        [NoDirectAccess]
        public IActionResult AddOrEdit(int? id, bool returnToDetailView = false)
        {
            LandTitleViewModel landTitleViewModel = GetLandTitleViewModel(id, returnToDetailView: returnToDetailView);
            return View(landTitleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(LandTitleViewModel landTitleViewModel)
        {
            LandTitleDto landTitleDto = landTitleViewModel.LandTitle;
            try
            {
                if (ModelState.IsValid)
                {
                    _landTitleCommand.CurrentUser = CurrentUser.UserName;
                    if (landTitleDto.Id == 0)
                    {
                        landTitleDto.Id = _landTitleCommand.Add(landTitleDto);
                    }
                    else
                    {
                        _landTitleCommand.Update(landTitleDto);
                    }
                    _landTitleCommand.Save();

                    string returnHtml;
                    if (landTitleViewModel.ReturnToDetailView)
                    {
                        landTitleViewModel = GetLandTitleViewModel(landTitleDto.Id, loadDocuments: true,
                            returnToDetailView: landTitleViewModel.ReturnToDetailView);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_Detail", landTitleViewModel);
                    }
                    else
                    {
                        var landTitleDtos = _landTitleQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_List", landTitleDtos);
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

                landTitleViewModel = GetLandTitleViewModel(landTitleDto.Id);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEdit", landTitleViewModel)
                });
            }
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _landTitleCommand.DbAction = DataBaseActionEnum.Delete;
                _landTitleCommand.Delete(id);
                _landTitleCommand.Save();
                return Json(new { success = true, message = "Opération effectuée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Json(new { success = false, message = $"Echec lors de l'exécution de l'opération : {ex.Message}" });
            }
        }

        public LandTitleViewModel GetLandTitleViewModel(int? landTitleId = null, bool loadDocuments = false, 
            bool returnToDetailView = false)
        {
            LandTitleDto landTitle = new();
            List<LandTitleDocumentDto> landTitleDocuments = new();
            if (landTitleId != null && landTitleId != 0)
            {
                landTitle = _landTitleQuery.GetById(landTitleId.GetValueOrDefault());
                if (loadDocuments)
                {
                    landTitleDocuments = _landTitleDocumentQuery.GetByLandTitleId(landTitleId.GetValueOrDefault()).ToList();
                }
            }

            IEnumerable<SelectListItem> localities = _localityQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.DisplayText,
                Value = i.Id.ToString()
            });

            LandTitleViewModel landTitleViewModel = new()
            {
                LandTitle = landTitle,
                LandTitleDocuments = landTitleDocuments,
                LocalityList = localities,
                ReturnToDetailView = returnToDetailView
            };

            return landTitleViewModel;
        }

        #endregion LandTitle

        #region LandTitleDocument

        [NoDirectAccess]
        public IActionResult AddOrEditDocument(int? id, int? landTitleId)
        {
            LandTitleDocumentViewModel landTitleDocumentViewModel = GetLandTitleDocumentViewModel(id, landTitleId);
            return View(landTitleDocumentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditDocument(LandTitleDocumentViewModel landTitleDocumentViewModel)
        {
            LandTitleDocumentDto landTitleDocumentDto = landTitleDocumentViewModel.LandTitleDocument;
            try
            {
                IFormFileCollection files = HttpContext.Request.Form.Files;
                if ((files.Count == 0 && landTitleDocumentDto.IsNew()) ||
                    (files.Count > 0 && !Path.GetExtension(files[0].FileName).ToLower().Equals(ConstantHelper.DEFAULT_DOCUMENT_EXTENSION)))
                {
                    ModelState.AddModelError("ModelError", "Vous devez sélectionner un fichier pdf.");
                }
                if (landTitleDocumentDto.DocumentTypeId == 0)
                {
                    ModelState.AddModelError("ModelError", "Le type de document est obligatoire.");
                }

                if (ModelState.IsValid)
                {
                    if (files.Count !=0)
                    {
                        landTitleDocumentDto.Document = files[0];

                        if (landTitleDocumentDto.Document != null)
                        {
                            landTitleDocumentDto.DocumentUrl = landTitleDocumentDto.Document.FileName;
                        }
                    }

                    _landTitleDocumentCommand.CurrentUser = CurrentUser.UserName;
                    if (landTitleDocumentDto.IsNew())
                    {
                        _landTitleDocumentCommand.Add(landTitleDocumentDto);
                    }
                    else
                    {
                        _landTitleDocumentCommand.Update(landTitleDocumentDto);
                    }
                    _landTitleDocumentCommand.Save();

                    LandTitleViewModel landTitleViewModel = new()
                    {
                        LandTitle = _landTitleQuery.GetById(landTitleDocumentViewModel.LandTitleDocument.LandTitleId.GetValueOrDefault()),
                        LandTitleDocuments = _landTitleDocumentQuery.GetByLandTitleId(landTitleDocumentViewModel.LandTitleDocument.LandTitleId.GetValueOrDefault())
                    };

                    return Json(new
                    {
                        isValid = true,
                        message = "Opération effectuée avec succès.",
                        html = MvcHelper.RenderRazorViewToString(this, "_Detail", landTitleViewModel)
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

                landTitleDocumentViewModel = GetLandTitleDocumentViewModel(landTitleDocumentDto.Id, landTitleDocumentDto.LandTitleId);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEditDocument", landTitleDocumentViewModel)
                });
            }
        }

        public IActionResult DeleteDocument(int id)
        {
            LandTitleDocumentDto landTitleDocumentDto = _landTitleDocumentQuery.GetById(id);
            if (landTitleDocumentDto == null)
            {
                return Json(new { success = false, message = "L'enregistrement a supprimer n'a pas été trouvé." });
            }
            _landTitleDocumentCommand.DbAction = DataBaseActionEnum.Delete;
            StringBuilder validationErrors = new();
            _landTitleDocumentCommand.Delete(id);
            if (validationErrors.Length != 0)
            {
                return Json(new { success = false, message = validationErrors.ToString() });
            }
            _landTitleDocumentCommand.Save();

            if (landTitleDocumentDto.DocumentUrl != null)
            {
                FileHelper.DeleteFile(DocumentOwnerEnum.LandTitle, _hostEnvironment.WebRootPath, landTitleDocumentDto.DocumentUrl);
            }
            return Json(new { success = true, message = "Opération effectuée avec succès" });
        }

        private LandTitleDocumentViewModel GetLandTitleDocumentViewModel(int? id = null, int? landTitleId = null)
        {
            LandTitleDocumentDto landTitleDocumentDto = new();

            if (id != null && id != 0)
            {
                landTitleDocumentDto = _landTitleDocumentQuery.GetById(id.GetValueOrDefault());
            }
            else if (landTitleId != null && landTitleId != 0)
            {
                landTitleDocumentDto = new LandTitleDocumentDto
                {
                    LandTitleId = landTitleId.GetValueOrDefault(),
                    LandTitle = _landTitleQuery.GetById(landTitleId.GetValueOrDefault())
                };
            }

            LandTitleDocumentViewModel landTitleDocumentViewModel = new()
            {
                LandTitleDocument = landTitleDocumentDto,
                DocumentTypeList = _documentTypeQuery.GetAll().Select(i => new SelectListItem
                {
                    Text = i.DisplayText,
                    Value = i.Id.ToString()
                })
            };
            return landTitleDocumentViewModel;
        }

        [HttpGet]
        public IActionResult DiplayDocument(string documentUrl)
        {
            return FileHelper.DiplayPDF(DocumentOwnerEnum.LandTitle, _hostEnvironment.WebRootPath, documentUrl);
        }

        #endregion LandTitleDocument
    }

}