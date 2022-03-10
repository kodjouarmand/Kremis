using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Domain.Entities;
using Kremis.Utility.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kremis.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IApplicationUserQuery _applicationUserQuery;
        

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IApplicationUserQuery applicationUserQuery)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _applicationUserQuery = applicationUserQuery;
            
        }

        public string UserName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Role { get; set; }

            public IEnumerable<SelectListItem> CompanyList { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }

        }

        private void Load(ApplicationUser user)
        {
            UserName = user.UserName;
            string roleName = _applicationUserQuery.GetRoleName(user);
            Input = new InputModel
            {
                Name = user.Name,

                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roleName,
                RoleList = _roleManager.Roles.Where(x => x.Name == roleName)
                    .Select(x => x.Name).Select(i => new SelectListItem
                    {
                        Text = i,
                        Value = i
                    })
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //await LoadAsync(user);
            Load(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                //await LoadAsync(user);
                Load(user);
                return Page();
            }

            user.Name = Input.Name;
            user.Email = Input.Email;
            user.PhoneNumber = Input.PhoneNumber;
            user.RoleName = Input.Role;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                string lastRoleName = _applicationUserQuery.GetRoleName(user);
                await _userManager.RemoveFromRoleAsync(user, lastRoleName);
                await _userManager.AddToRoleAsync(user, user.RoleName);
            }
            else
            {
                throw new InvalidOperationException($"Unexpected error occurred when updating user with ID '{user.UserName}'.");
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
