using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppIdentity.Models;
using AppIdentity.Data;
using Microsoft.AspNetCore.Authorization;
using AppIdentity.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Claims;
using AppIdentity.Areas.Identity.Pages.Account;

namespace AppIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppUsersDbContext _appUsersDbContext;
        private readonly AppUserManager _appUserManager;
        private readonly SignInManager<AppUser> _signInManager;
        private AppUser _loggedUser;


        public HomeController(AppUsersDbContext appUsersDbContext,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _appUsersDbContext = appUsersDbContext;
            _signInManager = signInManager;
            _appUserManager = new AppUserManager(_appUsersDbContext, userManager, signInManager);
        }
        private async Task<IActionResult> UserAct(List<string> checkedId, Action act)
        {
            _loggedUser = await _signInManager.UserManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_appUserManager.CheckProfile(_loggedUser))
            {
                return await UserThrow();
            }
            else
            {
                act();
                if (_appUserManager.CheckProfile(_loggedUser))
                {
                    return await UserThrow();
                }
                return RedirectToAction("Users");
            }
        }
        private async Task<IActionResult> UserThrow()
        {
            _loggedUser = await _signInManager.UserManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _appUserManager.LogoutUser(_loggedUser);
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        public async Task<IActionResult> Index()
        {
            if(User.FindFirstValue(ClaimTypes.NameIdentifier) == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            _loggedUser = await LoadUser();
            _loggedUser.Status = true;
            _appUsersDbContext.SaveChanges();
            return RedirectToAction("Users");
        }
        private bool CheckExistence()
        {
            if (_signInManager.UserManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)) == null)
            {
                return false;
            }
            return true;
        }
        private async Task<AppUser> LoadUser()
        {
            return await _signInManager.UserManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        [Authorize]
        public async Task<IActionResult> Users()
        {
            if(User.FindFirstValue(ClaimTypes.NameIdentifier) == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }    
            _loggedUser = await LoadUser();
            if (_loggedUser == null || _appUserManager.CheckProfile(_loggedUser))
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            ViewBag.Users = _appUsersDbContext.Users;
            return View();
        } 
        
        public async Task<IActionResult> Delete(List<string> checkedId)
        {
            return await UserAct(checkedId, () =>
            {
                _appUserManager.DeleteUser(checkedId);
            });
        }

        public async Task<IActionResult> Block(List<string> checkedId)
        {
            return await UserAct(checkedId, () =>
            {
                _appUserManager.BlockUser(checkedId);
            });
        }
        

        public async Task<IActionResult> Unblock(List<string> checkedId)
        {
            return await UserAct(checkedId, () =>
            {
                _appUserManager.UnblockUser(checkedId);
            });
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
