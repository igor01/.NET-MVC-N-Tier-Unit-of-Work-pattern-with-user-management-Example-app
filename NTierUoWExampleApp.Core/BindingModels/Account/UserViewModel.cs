using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.Account
{
    public class UserViewModel
    {
        public UserViewModel()
        {

        }

        public UserViewModel(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;
            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            TwoFactorEnabled = user.TwoFactorEnabled;
            LockoutEndDateUtc = user.LockoutEndDateUtc;
            LockoutEnabled = user.LockoutEnabled;
            AccessFailedCount = user.AccessFailedCount;
            PasswordHash = user.PasswordHash;
            SecurityStamp = user.SecurityStamp;
            FirstName = user.FirstName;
            LastName = user.LastName;
            CreatedBy = user.CreatedBy;
            Status = user.Status;
            OnlineStatus = user.OnlineStatus;
            NotificationEnabled = user.NotificationEnabled;
            CreatedDate = user.CreatedDate;
            LastLogin = user.LastLogin;
            LastLoginUtc = user.LastLoginUtc;
            ModifiedBy = user.ModifiedBy;
            ModifiedDate = user.ModifiedDate;
            PermanentLock = user.PermanentLock;
        }


        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string Status { get; set; }
        public virtual string OnlineStatus { get; set; }
        public virtual bool NotificationEnabled { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        public virtual DateTime? LastLoginUtc { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool PermanentLock { get; set; }
    }
}
