using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AppIdentity.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using AppIdentity.Data;

namespace AppIdentity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly AppUsersDbContext _appUsersDbContext;

        public LogoutModel(SignInManager<AppUser> signInManager, ILogger<LogoutModel> logger,
            AppUsersDbContext appUsersDbContext)
        {
            _signInManager = signInManager;
            _logger = logger;
            _appUsersDbContext = appUsersDbContext;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            _signInManager.UserManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result.Status = false;
            _signInManager.UserManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result.LastVisit = DateTime.Now;
            await _appUsersDbContext.SaveChangesAsync();
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
