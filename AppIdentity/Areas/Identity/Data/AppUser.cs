using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AppIdentity.Areas.Identity.Data
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastVisit { get; set; }

        public bool Status { get; set; }

        public bool Banned { get; set; }
    }
}
