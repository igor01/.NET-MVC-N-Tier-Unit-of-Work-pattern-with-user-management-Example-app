using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.ModelsConfiguration.Account
{
    public class UserNotificationConfig : EntityTypeConfiguration<UserNotification>
    {
        public UserNotificationConfig()
        {
            HasKey(p => p.UserNotificationId);
            Property(p => p.UserNotificationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(x => x.User)
                .WithMany(x => x.UserNotifications)
                .HasForeignKey(x => x.UserId);
        }
    }
}
