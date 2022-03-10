using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kremis.Utility.Helpers
{
    public class IdentityHelper
    {
        public static bool IsAdministrator(ClaimsPrincipal user)
        {
            return IsSuperAdministrator(user) || user.IsInRole(ConstantHelper.ROLE_NAME_ADMIN);
        }
        public static bool IsSuperAdministrator(ClaimsPrincipal user)
        {
            return user.IsInRole(ConstantHelper.ROLE_NAME_SUPER_ADMIN);
        }
    }
}
