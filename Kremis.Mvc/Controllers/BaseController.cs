using System;
using System.Security.Claims;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Domain.Entities;
using Kremis.Infrastructure.Contracts;
using Kremis.Utility.Helpers;
using Kremis.Utility.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kremis.Mvc.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly ILoggerService _logger;
        protected readonly IApplicationUserQuery _applicationUserQuery;
        public BaseController(ILoggerService logger, IApplicationUserQuery applicationUserQuery)
        {
            _logger = logger;
            _applicationUserQuery = applicationUserQuery;
        }

        public ApplicationUser CurrentUser
        {
            get => _applicationUserQuery.GetCurrentUser((ClaimsIdentity)User.Identity);
        }

        protected void CheckActionAuthorization()
        {
            if (!IdentityHelper.IsAdministrator(User))
            {
                throw new Exception("Vous n'êtes pas autorisé à effectuer cette opération.");
            }
        }
        
}
}
