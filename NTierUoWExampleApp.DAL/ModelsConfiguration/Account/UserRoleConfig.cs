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
    public class UserRoleConfig : EntityTypeConfiguration<UserRole>
    {
        public UserRoleConfig()
        {
            HasKey(p => p.UserRoleId);
            Property(p => p.UserRoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
