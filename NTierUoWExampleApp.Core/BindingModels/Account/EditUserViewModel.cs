using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.Account
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {

        }

        public EditUserViewModel(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Locked = (user.LockoutEndDateUtc != null);
        }

        public string Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Compare("UserName", ErrorMessage = "The Username and Email do not match.")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public bool Locked { get; set; }

        public string ModifiedBy { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string UserAccessRole { get; set; }
        public List<string> UserAccessRoles { get; set; }
    }
}
