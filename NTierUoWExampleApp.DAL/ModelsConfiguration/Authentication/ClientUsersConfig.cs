using NTierUoWExampleApp.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.ModelsConfiguration.Authentication
{
    public class ClientUsersConfig : EntityTypeConfiguration<ClientUsers>
    {
        public ClientUsersConfig()
        {
            ToTable("ClientUsers");
            HasKey(t => t.ClientUsersId);
            Property(t => t.ClientUsersId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            HasRequired(t => t.User)
                .WithMany(t => t.ClientUsers)
                .HasForeignKey(t => t.UserId);

            HasRequired(t => t.Client)
                .WithMany(t => t.ClientUsers)
                .HasForeignKey(t => t.ClientId);
        }
    }
}
