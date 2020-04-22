using System;
using AppIdentity.Areas.Identity.Data;
using AppIdentity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AppIdentity.Areas.Identity.IdentityHostingStartup))]
namespace AppIdentity.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AppUsersDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AppUsersDbContextConnection")));

                services.AddDefaultIdentity<AppUser>()
                    .AddEntityFrameworkStores<AppUsersDbContext>();
            });
        }
    }
}