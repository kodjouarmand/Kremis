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
    public class ParcelController : BaseAdminController
    {
        private readonly IParcelQuery _parcelQuery;
        private readonly IParcelCommand _parcelCommand;
        private readonly ILandTitleQuery _landTitleQuery;
        private readonly IParcelDocumentQuery _parcelDocumentQuery;
        private readonly IParcelDocumentCommand _parcelDocumentCommand;
        private readonly IDocumentTypeQuery _documentTypeQuery;
        private readonly ILocalityQuery _localityQuery;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ParcelController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            IParcelQuery parcelQuery, IParcelCommand parcelCommand, ILandTitleQuery landTitleQuery,
            IParcelDocumentQuery parcelDocumentQuery, IParcelDocumentCommand parcelDocumentCommand,
            IDocumentTypeQuery documentTypeQuery, ILocalityQuery localityQuery,
            IWebHostEnvironment hostEnvironment)
            : base(logger, applicationUserQuery)
        {
            _parcelQuery = parcelQuery;
            _parcelCommand = parcelCommand;
            _landTitleQuery = landTitleQuery;
            _parcelDocumentQuery = parcelDocumentQuery;
            _parcelDocumentCommand = parcelDocumentCommand;
            _documentTypeQuery = documentTypeQuery;
            _localityQuery = localityQuery;
            _hostEnvironment = hostEnvironment;
        }

        #region Parcel

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var parcelDtos = _parcelQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(parcelDtos);
        }

        [NoDirectAccess]
        public IActionResult Summary(int id)
        {
            ParcelViewModel parcelViewModel = GetParcelViewModel(id, true);
            return View(parcelViewModel);
        }

        [NoDirectAccess]
        public IActionResult AddOrEdit(int? id, bool returnToDetailView = false)
        {
            ParcelViewModel parcelViewModel = GetParcelViewModel(id, returnToDetailView: returnToDetailView);
            return View(parcelViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(ParcelViewModel parcelViewModel)
        {
            ParcelDto parcelDto = parcelViewModel.Parcel;
            try
            {
                if (ModelState.IsValid)
                {
                    _parcelCommand.CurrentUser = CurrentUser.UserName;
                    if (parcelDto.Id == 0)
                    {
                        parcelDto.Id = _parcelCommand.Add(parcelDto);
                    }
                    else
                    {
                        _parcelCommand.Update(parcelDto);
                    }
                    _parcelCommand.Save();

                    string returnHtml;
                    if (parcelViewModel.ReturnToDetailView)
                    {
                        parcelViewModel = GetParcelViewModel(parcelDto.Id, loadDocuments: true,
                            returnToDetailView: parcelViewModel.ReturnToDetailView);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_Detail", parcelViewModel);
                    }
                    else
                    {
                        var parcelDtos = _parcelQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_List", parcelDtos);
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

                parcelViewModel = GetParcelViewModel(parcelDto.Id);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEdit", parcelViewModel)
                });
            }
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _parcelCommand.DbAction = DataBaseActionEnum.Delete;
                _parcelCommand.Delete(id);
                _parcelCommand.Save();
                return Json(new { success = true, message = "Opération effectuée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Json(new { success = false, message = $"Echec lors de l'exécution de l'opération : {ex.Message}" });
            }
        }

        private ParcelViewModel GetParcelViewModel(int? parcelId = null, bool loadDocuments = false,
            bool returnToDetailView = false)
        {
            ParcelDto parcelDto = new();
            List<ParcelDocumentDto> parcelDocuments = new();
            if (parcelId != null && parcelId != 0)
            {
                parcelDto = _parcelQuery.GetById(parcelId.GetValueOrDefault());
                if (loadDocuments)
                {
                    parcelDocuments = _parcelDocumentQuery.GetByParcelId(parcelId.GetValueOrDefault()).ToList();
                }
            }

            IEnumerable<SelectListItem> landTitles = _landTitleQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.DisplayText,
                Value = i.Id.ToString()
            });

            IEnumerable<SelectListItem> loclities = _localityQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.DisplayText,
                Value = i.Id.ToString()
            });

            ParcelViewModel parcelViewModel = new()
            {
                Parcel = parcelDto,
                ParcelDocuments = parcelDocuments,
                LandTitleList = landTitles,
                LocalityList = loclities,
                ReturnToDetailView = returnToDetailView
            };
            return parcelViewModel;
        }

        [HttpGet]
        public IActionResult GetLandTitle(int landTitleId)
        {
            LandTitleDto landTitleDto = _landTitleQuery.GetById(landTitleId);
            if (landTitleDto == null)
            {
                landTitleDto = new LandTitleDto();
            }

            return Json(landTitleDto);
        }

        #endregion Parcel

        #region ParcelDocument

        [NoDirectAccess]
        public IActionResult AddOrEditDocument(int? id, int? parcelId)
        {
            ParcelDocumentViewModel parcelDocumentViewModel = GetParcelDocumentViewModel(id, parcelId);
            return View(parcelDocumentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditDocument(ParcelDocumentViewModel parcelDocumentViewModel)
        {
            ParcelDocumentDto parcelDocumentDto = parcelDocumentViewModel.ParcelDocument;
            try
            {
                IFormFileCollection files = HttpContext.Request.Form.Files;
                if ((files.Count == 0 && parcelDocumentDto.IsNew()) ||
                    (files.Count > 0 && !Path.GetExtension(files[0].FileName).ToLower().Equals(ConstantHelper.DEFAULT_DOCUMENT_EXTENSION)))
                {
                    ModelState.AddModelError("ModelError", "Vous devez sélectionner un fichier pdf.");
                }
                if (parcelDocumentDto.DocumentTypeId == 0)
                {
                    ModelState.AddModelError("ModelError", "Le type de document est obligatoire.");
                }

                if (ModelState.IsValid)
                {
                    if (files.Count != 0)
                    {
                        parcelDocumentDto.Document = files[0];

                        if (parcelDocumentDto.Document != null)
                        {
                            parcelDocumentDto.DocumentUrl = parcelDocumentDto.Document.FileName;
                        }
                    }

                    _parcelDocumentCommand.CurrentUser = CurrentUser.UserName;
                    if (parcelDocumentDto.IsNew())
                    {
                        _parcelDocumentCommand.Add(parcelDocumentDto);
                    }
                    else
                    {
                        _parcelDocumentCommand.Update(parcelDocumentDto);
                    }
                    _parcelDocumentCommand.Save();

                    ParcelViewModel parcelViewModel = new()
                    {
                        Parcel = _parcelQuery.GetById(parcelDocumentViewModel.ParcelDocument.ParcelId.GetValueOrDefault()),
                        ParcelDocuments = _parcelDocumentQuery.GetByParcelId(parcelDocumentViewModel.ParcelDocument.ParcelId.GetValueOrDefault())
                    };

                    return Json(new
                    {
                        isValid = true,
                        message = "Opération effectuée avec succès.",
                        html = MvcHelper.RenderRazorViewToString(this, "_Detail", parcelViewModel)
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

                parcelDocumentViewModel = GetParcelDocumentViewModel(parcelDocumentDto.Id, parcelDocumentDto.ParcelId);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEditDocument", parcelDocumentViewModel)
                });
            }
        }

        public IActionResult DeleteDocument(int id)
        {
            ParcelDocumentDto parcelDocumentDto = _parcelDocumentQuery.GetById(id);
            if (parcelDocumentDto == null)
            {
                return Json(new { success = false, message = "L'enregistrement a supprimer n'a pas été trouvé." });
            }
            _parcelDocumentCommand.DbAction = DataBaseActionEnum.Delete;
            StringBuilder validationErrors = new();
            _parcelDocumentCommand.Delete(id);
            if (validationErrors.Length != 0)
            {
                return Json(new { success = false, message = validationErrors.ToString() });
            }
            _parcelDocumentCommand.Save();

            if (parcelDocumentDto.DocumentUrl != null)
            {
                FileHelper.DeleteFile(DocumentOwnerEnum.Parcel, _hostEnvironment.WebRootPath, parcelDocumentDto.DocumentUrl);
            }
            return Json(new { success = true, message = "Opération effectuée avec succès" });
        }

        private ParcelDocumentViewModel GetParcelDocumentViewModel(int? id = null, int? parcelId = null)
        {
            ParcelDocumentDto parcelDocumentDto = new();

            if (id != null && id != 0)
            {
                parcelDocumentDto = _parcelDocumentQuery.GetById(id.GetValueOrDefault());
            }
            else if (parcelId != null && parcelId != 0)
            {
                parcelDocumentDto = new ParcelDocumentDto
                {
                    ParcelId = parcelId.GetValueOrDefault(),
                    Parcel = _parcelQuery.GetById(parcelId.GetValueOrDefault())
                };
            }

            ParcelDocumentViewModel parcelDocumentViewModel = new()
            {
                ParcelDocument = parcelDocumentDto,
                DocumentTypeList = _documentTypeQuery.GetAll().Select(i => new SelectListItem
                {
                    Text = i.DisplayText,
                    Value = i.Id.ToString()
                })
            };
            return parcelDocumentViewModel;
        }

        [HttpGet]
        public IActionResult DiplayDocument(string documentUrl)
        {
            return FileHelper.DiplayPDF(DocumentOwnerEnum.Parcel, _hostEnvironment.WebRootPath, documentUrl);
        }


        #endregion ParcelDocument
    }

}