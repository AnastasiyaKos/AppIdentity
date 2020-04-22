using AppIdentity.Areas.Identity.Data;
using AppIdentity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppIdentity.Models
{
    public class AppUserManager
    {

        private readonly AppUsersDbContext _appUsersDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;



        public AppUserManager(AppUsersDbContext appUsersDbContext,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _appUsersDbContext = appUsersDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        

        public bool CheckProfile(AppUser appUser)
        {
            AppUser user = appUser;
            if (_appUsersDbContext.Users.Find(user.Id) == null || _appUsersDbContext.Users.Find(user.Id).Banned)
            {
                return true;
            }
            return false;
        }

        public async Task LogoutUser(AppUser user)
        {
            user.Status = false;
            user.LastVisit = DateTime.Now;
            _appUsersDbContext.SaveChanges();
            await _signInManager.SignOutAsync();
        }

        private void func(List<string> list, Action<AppUser> filter)
        {
            foreach (AppUser user in _appUsersDbContext.Users)
            {
                if (list.Contains(user.Id))
                {
                    filter.Invoke(user);
                }
            }
            _appUsersDbContext.SaveChanges();
        }
        public void DeleteUser(List<string> members)
        {
            func(members, (user) =>
            {
                _appUsersDbContext.Users.Remove(user);
            });
        }
        public void BlockUser(List<string> members)
        {
            func(members, (user) =>
            {
                _appUsersDbContext.Users.Update(user).Entity.Banned = true;
                _appUsersDbContext.Users.Update(user).Entity.Status = false;
                _appUsersDbContext.Users.Update(user).Entity.LastVisit = DateTime.Now;
            });    
        }
        public void UnblockUser(List<string> members)
        {
            func(members, (user) =>
            {
                _appUsersDbContext.Users.Update(user).Entity.Banned = false;
            });
        }
    }
}
