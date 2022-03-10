using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kremis.BusinessLogic.Queries.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Kremis.Mvc.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly IApplicationUserQuery _applicationUserQuery;

        public UserNameViewComponent(IApplicationUserQuery applicationUserQuery)
        {
            _applicationUserQuery = applicationUserQuery;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Domain.Entities.ApplicationUser user = _applicationUserQuery.GetCurrentUser((ClaimsIdentity)User.Identity);
            return View(user);
        }
    }
}
