using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Infrastructure.Contracts;
using Kremis.Mvc.Controllers;
using Kremis.Utility.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Kremis.Mvc
{
    [Area("Admin")]
    [Authorize(Roles = ConstantHelper.ROLE_NAME_SUPER_ADMIN + ", " + ConstantHelper.ROLE_NAME_ADMIN)]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(ILoggerService logger, IApplicationUserQuery applicationUserQuery)
            : base(logger, applicationUserQuery) { }
    }
}
