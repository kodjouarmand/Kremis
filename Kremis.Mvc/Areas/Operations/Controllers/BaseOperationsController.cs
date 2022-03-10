using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Infrastructure.Contracts;
using Kremis.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Kremis.Mvc
{
    [Area("Operations")]
    public class BaseOperationsController : BaseController
    {
        public BaseOperationsController(ILoggerService logger, IApplicationUserQuery applicationUserQuery)
            : base(logger, applicationUserQuery) { }
    }
}
