using System;
using System.Diagnostics;
using System.Linq;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Domain.Paging;
using Kremis.Domain.ViewModels;
using Kremis.Infrastructure.Contracts;
using Kremis.Utility.Enum;
using Kremis.Utility.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Kremis.Mvc.Controllers
{
    [Area("Home")]
    public class HomeController : BaseController
    {
        private readonly IInvoiceHeaderQuery _invoiceHeaderQuery;
        private readonly IParcelQuery _parcelQuery;
        private readonly ICustomerQuery _customerQuery;

        public HomeController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            IInvoiceHeaderQuery invoiceHeaderQuery, IParcelQuery parcelQuery, ICustomerQuery customerQuery)
            : base(logger, applicationUserQuery)
        {
            _invoiceHeaderQuery = invoiceHeaderQuery;
            _customerQuery = customerQuery;
            _parcelQuery = parcelQuery;
        }

        public IActionResult Index(int page = 1)
        {
            if (CurrentUser == null)
                return Redirect("/Identity/Account/Login");

            var invoiceDtos = _invoiceHeaderQuery.GetAll();
            var unpaidInvoiceDtos = invoiceDtos.Where(u => u.Status != EnumHelper.ToString(StatusEnum.Paid));
            var overdueInvoiceDtos = invoiceDtos.Where(u => u.PaymentDueDate < DateTime.Today.Date);

            var availableParcelDtos = _parcelQuery.GetAll().Where(u => u.Status == EnumHelper.ToString(StatusEnum.Available));
            var debtorCustomerDtos = unpaidInvoiceDtos.GroupBy(u => u.CustomerId).Select(u => u.First().Customer);            
            foreach (var customer in debtorCustomerDtos)
            {
                customer.AccountBalance = unpaidInvoiceDtos.Where(u => u.CustomerId == customer.Id)
                    .Sum(u => u.RemainingAmountToPay);
            }

            var availableParcels = availableParcelDtos.AsQueryable().GetPaged(page, 3);
            var advancedInvoices = overdueInvoiceDtos.AsQueryable().GetPaged(page, 3);
            var overdueInvoices = unpaidInvoiceDtos.AsQueryable().GetPaged(page, 3);
            var debtorCustomers = debtorCustomerDtos.AsQueryable().GetPaged(page, 3);

            DashboardViewModel dashboardViewModel = new()
            {
                UnpaidInvoices = overdueInvoices,
                OverdueInvoices = advancedInvoices,
                AvailableParcels = availableParcels,
                DebtorCustomers = debtorCustomers
            };

            return View(dashboardViewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ErrorViewModel error = new()
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            IExceptionHandlerFeature contextFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature != null)
            {
                _logger.LogError(contextFeature.Error);
                error.Message = contextFeature.Error.Message;
            }

            return View(error);
        }
    }
}
