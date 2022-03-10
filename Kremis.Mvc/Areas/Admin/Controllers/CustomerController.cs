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
    public class CustomerController : BaseAdminController
    {
        private readonly ICustomerQuery _customerQuery;
        private readonly ICustomerCommand _customerCommand;
        private readonly IIdentityDocumentTypeQuery _identityDocumentTypeQuery;
        private readonly ICustomerDocumentQuery _customerDocumentQuery;
        private readonly ICustomerDocumentCommand _customerDocumentCommand;
        private readonly IDocumentTypeQuery _documentTypeQuery;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CustomerController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            ICustomerQuery customerQuery, ICustomerCommand customerCommand, IIdentityDocumentTypeQuery identityDocumentTypeQuery,
            ICustomerDocumentQuery customerDocumentQuery, ICustomerDocumentCommand customerDocumentCommand,
            IDocumentTypeQuery documentTypeQuery, IWebHostEnvironment hostEnvironment)
            : base(logger, applicationUserQuery)
        {
            _customerQuery = customerQuery;
            _customerCommand = customerCommand;
            _identityDocumentTypeQuery = identityDocumentTypeQuery;
            _customerDocumentCommand = customerDocumentCommand;
            _customerDocumentQuery = customerDocumentQuery;
            _documentTypeQuery = documentTypeQuery;
            _hostEnvironment = hostEnvironment;
        }

        #region Customer

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var customerDtos = _customerQuery.GetAll().AsQueryable().GetPaged(page, pageSize);
            return View(customerDtos);
        }

        [NoDirectAccess]
        public IActionResult Summary(int id)
        {
            CustomerViewModel customerViewModel = GetCustomerViewModel(id, true);
            return View(customerViewModel);
        }

        [NoDirectAccess]
        public IActionResult AddOrEdit(int? id, bool returnToDetailView = false)
        {
            CustomerViewModel customerViewModel = GetCustomerViewModel(id, returnToDetailView: returnToDetailView);
            return View(customerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(CustomerViewModel customerViewModel)
        {
            CustomerDto customerDto = customerViewModel.Customer;
            try
            {
                if (ModelState.IsValid)
                {
                    _customerCommand.CurrentUser = CurrentUser.UserName;
                    if (customerDto.Id == 0)
                    {
                        customerDto.Id = _customerCommand.Add(customerDto);
                    }
                    else
                    {
                        _customerCommand.Update(customerDto);
                    }
                    _customerCommand.Save();

                    string returnHtml;
                    if (customerViewModel.ReturnToDetailView)
                    {
                        customerViewModel = GetCustomerViewModel(customerDto.Id, loadDocuments: true,
                            returnToDetailView: customerViewModel.ReturnToDetailView);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_Detail", customerViewModel);
                    }
                    else
                    {
                        var customerDtos = _customerQuery.GetAll().AsQueryable().GetPaged(1, ConstantHelper.DEFAULT_PAGE_SIZE);
                        returnHtml = MvcHelper.RenderRazorViewToString(this, "_List", customerDtos);
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

                customerViewModel = GetCustomerViewModel(customerDto.Id);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEdit", customerViewModel)
                });
            }
        }

        //[HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                CheckActionAuthorization();

                _customerCommand.DbAction = DataBaseActionEnum.Delete;
                _customerCommand.Delete(id);
                _customerCommand.Save();
                return Json(new { success = true, message = "Opération effectuée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return Json(new { success = false, message = $"Echec lors de l'exécution de l'opération : {ex.Message}" });
            }
        }

        private CustomerViewModel GetCustomerViewModel(int? customerId = null, bool loadDocuments = false,
            bool returnToDetailView = false)
        {
            CustomerDto customer = new();
            List<CustomerDocumentDto> customerDocuments = new();
            if (customerId != null && customerId != 0)
            {
                customer = _customerQuery.GetById(customerId.GetValueOrDefault());
                if (loadDocuments)
                {
                    customerDocuments = _customerDocumentQuery.GetByCustomerId(customerId.GetValueOrDefault()).ToList();
                }
            }

            IEnumerable<SelectListItem> identityDocumentTypes = _identityDocumentTypeQuery.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            CustomerViewModel customerViewModel = new()
            {
                Customer = customer,
                CustomerDocuments = customerDocuments,
                IdentityDocumentTypeList = identityDocumentTypes,
                ReturnToDetailView = returnToDetailView
            };
            return customerViewModel;

        }
        
        #endregion Customer

        #region CustomerDocument

        [NoDirectAccess]
        public IActionResult AddOrEditDocument(int? id, int? customerId)
        {
            CustomerDocumentViewModel customerDocumentViewModel = GetCustomerDocumentViewModel(id, customerId);
            return View(customerDocumentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditDocument(CustomerDocumentViewModel customerDocumentViewModel)
        {
            CustomerDocumentDto customerDocumentDto = customerDocumentViewModel.CustomerDocument;
            try
            {
                IFormFileCollection files = HttpContext.Request.Form.Files;
                if ((files.Count == 0 && customerDocumentDto.IsNew()) ||
                    (files.Count > 0 && !Path.GetExtension(files[0].FileName).ToLower().Equals(ConstantHelper.DEFAULT_DOCUMENT_EXTENSION)))
                {
                    ModelState.AddModelError("ModelError", "Vous devez sélectionner un fichier pdf.");
                }
                if (customerDocumentDto.DocumentTypeId == 0)
                {
                    ModelState.AddModelError("ModelError", "Le type de document est obligatoire.");
                }

                if (ModelState.IsValid)
                {
                    if (files.Count != 0)
                    {
                        customerDocumentDto.Document = files[0];

                        if (customerDocumentDto.Document != null)
                        {
                            customerDocumentDto.DocumentUrl = customerDocumentDto.Document.FileName;
                        }
                    }

                    _customerDocumentCommand.CurrentUser = CurrentUser.UserName;
                    if (customerDocumentDto.IsNew())
                    {
                        _customerDocumentCommand.Add(customerDocumentDto);
                    }
                    else
                    {
                        _customerDocumentCommand.Update(customerDocumentDto);
                    }
                    _customerDocumentCommand.Save();

                    CustomerViewModel customerViewModel = new()
                    {
                        Customer = _customerQuery.GetById(customerDocumentViewModel.CustomerDocument.CustomerId.GetValueOrDefault()),
                        CustomerDocuments = _customerDocumentQuery.GetByCustomerId(customerDocumentViewModel.CustomerDocument.CustomerId.GetValueOrDefault())
                    };

                    return Json(new
                    {
                        isValid = true,
                        message = "Opération effectuée avec succès.",
                        html = MvcHelper.RenderRazorViewToString(this, "_Detail", customerViewModel)
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

                customerDocumentViewModel = GetCustomerDocumentViewModel(customerDocumentDto.Id, customerDocumentDto.CustomerId);
                return Json(new
                {
                    isValid = false,
                    message = ex.Message,
                    html = MvcHelper.RenderRazorViewToString(this, "AddOrEditDocument", customerDocumentViewModel)
                });
            }
        }

        public IActionResult DeleteDocument(int id)
        {
            CustomerDocumentDto customerDocumentDto = _customerDocumentQuery.GetById(id);
            if (customerDocumentDto == null)
            {
                return Json(new { success = false, message = "L'enregistrement a supprimer n'a pas été trouvé." });
            }
            _customerDocumentCommand.DbAction = DataBaseActionEnum.Delete;
            StringBuilder validationErrors = new();
            _customerDocumentCommand.Delete(id);
            if (validationErrors.Length != 0)
            {
                return Json(new { success = false, message = validationErrors.ToString() });
            }
            _customerDocumentCommand.Save();

            if (customerDocumentDto.DocumentUrl != null)
            {
                FileHelper.DeleteFile(DocumentOwnerEnum.Customer, _hostEnvironment.WebRootPath, customerDocumentDto.DocumentUrl);
            }
            return Json(new { success = true, message = "Opération effectuée avec succès" });
        }

        private CustomerDocumentViewModel GetCustomerDocumentViewModel(int? id = null, int? customerId = null)
        {
            CustomerDocumentDto customerDocumentDto = new();

            if (id != null && id != 0)
            {
                customerDocumentDto = _customerDocumentQuery.GetById(id.GetValueOrDefault());
            }
            else if (customerId != null && customerId != 0)
            {
                customerDocumentDto = new CustomerDocumentDto
                {
                    CustomerId = customerId.GetValueOrDefault(),
                    Customer = _customerQuery.GetById(customerId.GetValueOrDefault())
                };
            }

            CustomerDocumentViewModel customerDocumentViewModel = new()
            {
                CustomerDocument = customerDocumentDto,
                DocumentTypeList = _documentTypeQuery.GetAll().Select(i => new SelectListItem
                {
                    Text = i.DisplayText,
                    Value = i.Id.ToString()
                })
            };
            return customerDocumentViewModel;
        }

        [HttpGet]
        public IActionResult DiplayDocument(string documentUrl)
        {
            return FileHelper.DiplayPDF(DocumentOwnerEnum.Customer, _hostEnvironment.WebRootPath, documentUrl);
        }


        #endregion CustomerDocument
    }

}