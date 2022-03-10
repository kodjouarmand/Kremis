using Microsoft.AspNetCore.Mvc;
using Kremis.Infrastructure.Contracts;
using Kremis.Domain.Assemblers;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Mvc;
using Kremis.Domain.ViewModels;
using System.Linq;
using System.Collections.Generic;
using Kremis.Utility.Helpers;
using Kremis.Domain.Paging;

namespace Kremis.Areas.Operations.Controllers
{
    public class CommissionController : BaseOperationsController
    {
        private readonly IInvoiceHeaderQuery _invoiceHeaderQuery;
        private readonly IInvoiceDetailQuery _invoiceDetailQuery;

        public CommissionController(ILoggerService logger, IApplicationUserQuery applicationUserQuery,
            IInvoiceHeaderQuery invoiceHeaderQuery, IInvoiceDetailQuery invoiceDetailQuery)
            : base(logger, applicationUserQuery)
        {
            _invoiceHeaderQuery = invoiceHeaderQuery;
            _invoiceDetailQuery = invoiceDetailQuery;
        }

        public IActionResult Index(int page = 1, int pageSize = ConstantHelper.DEFAULT_PAGE_SIZE)
        {
            var commissions = _invoiceHeaderQuery.GetCommissions().AsQueryable().GetPaged(page, pageSize);
            return View(commissions);
        }


        public IActionResult Summary(int id)
        {
            InvoiceHeaderViewModel invoiceHeaderViewModel = GetInvoiceHeaderViewModel(id);
            return View(invoiceHeaderViewModel);
        }        

        private InvoiceHeaderViewModel GetInvoiceHeaderViewModel(int? invoiceHeaderId = null)
        {
            InvoiceHeaderDto invoiceHeaderDto = new();
            List<InvoiceDetailDto> invoiceDetailDtos = new();
            if (invoiceHeaderId != null && invoiceHeaderId != 0)
            {
                invoiceHeaderDto = _invoiceHeaderQuery.GetById(invoiceHeaderId.GetValueOrDefault());
                invoiceDetailDtos = _invoiceDetailQuery.GetByInvoiceHeaderId(invoiceHeaderId.GetValueOrDefault()).ToList();
            }

            InvoiceHeaderViewModel invoiceHeaderViewModel = new()
            {
                InvoiceHeader = invoiceHeaderDto,
                InvoiceDetails = invoiceDetailDtos
            };
            return invoiceHeaderViewModel;
        }
    }

}