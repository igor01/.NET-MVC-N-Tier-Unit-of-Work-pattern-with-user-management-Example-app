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
    public class ClientConfig : EntityTypeConfiguration<Client>
    {
        public ClientConfig()
        {
            HasKey(t => t.ClientId);
            Property(t => t.ClientId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
