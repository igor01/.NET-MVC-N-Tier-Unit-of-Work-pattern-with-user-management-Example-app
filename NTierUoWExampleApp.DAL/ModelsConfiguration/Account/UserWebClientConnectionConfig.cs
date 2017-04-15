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
    public class UserWebClientConnectionConfig : EntityTypeConfiguration<UserWebClientConnection>
    {
        public UserWebClientConnectionConfig()
        {
            HasKey(p => p.UserWebClientConnectionId);
            Property(p => p.UserWebClientConnectionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(x => x.User)
                .WithMany(x => x.UserWebClientConnections)
                .HasForeignKey(x => x.UserId);
        }
    }
}
