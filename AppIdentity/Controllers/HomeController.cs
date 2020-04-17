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

namespace AppIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppUsersDbContext _appUsersDbContext;

        public HomeController(ILogger<HomeController> logger, AppUsersDbContext appUsersDbContext)
        {
            _logger = logger;
            _appUsersDbContext = appUsersDbContext;
        }

        public IActionResult Index()
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Users()
        {
            ViewBag.Users = _appUsersDbContext.Users;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
