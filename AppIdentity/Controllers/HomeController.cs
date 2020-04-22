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

namespace AppIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppUsersDbContext _appUsersDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;



        public HomeController(ILogger<HomeController> logger, AppUsersDbContext appUsersDbContext, 
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _appUsersDbContext = appUsersDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if(User.FindFirstValue(ClaimTypes.NameIdentifier) == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            } 

            return RedirectToAction("Users");
        }

        [Authorize]
        public IActionResult Users()
        {
            ViewBag.Users = _appUsersDbContext.Users;
            return View();
        }

        public async Task<IActionResult> Delete(List<string> checkedId)
        {
            foreach(AppUser user in _appUsersDbContext.Users)
            {
                if(checkedId.Contains(user.Id))
                {
                    _appUsersDbContext.Users.Remove(user);
                }
            }

            _appUsersDbContext.SaveChanges();

            if(checkedId.Contains(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            } 

            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Block(List<string> checkedId)
        {
            foreach (AppUser user in _appUsersDbContext.Users)
            {
                if (checkedId.Contains(user.Id))
                {
                    _appUsersDbContext.Users.Update(user).Entity.Banned = true;
                    _appUsersDbContext.Users.Update(user).Entity.Status = false;
                    _appUsersDbContext.Users.Update(user).Entity.LastVisit = DateTime.Now;

                }
            }

            _appUsersDbContext.SaveChanges();

            if (checkedId.Contains(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            return RedirectToAction("Users");
        }

        public IActionResult Unblock(List<string> checkedId)
        {
            foreach (AppUser user in _appUsersDbContext.Users)
            {
                if (checkedId.Contains(user.Id))
                {
                    _appUsersDbContext.Users.Update(user).Entity.Banned = false;
                }
            }

            _appUsersDbContext.SaveChanges(); 

            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult Test(List<string> checkedId)
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return RedirectToPage("/Account/Logout", new { area = "Identity" } );
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
