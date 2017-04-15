using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.Account
{
    public class CreateUserViewModel
    {
        [Required]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string UserAccessRole { get; set; }
        public List<string> UserAccessRoles { get; set; }
    }
}
