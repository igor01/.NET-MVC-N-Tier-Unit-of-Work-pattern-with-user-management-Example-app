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
    public class BrowsingHistoryConfig : EntityTypeConfiguration<BrowsingHistory>
    {
        public BrowsingHistoryConfig()
        {
            ToTable("BrowsingHistory");
            HasKey(p => p.BrowsingHistoryId);
            Property(p => p.BrowsingHistoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(x => x.User)
                .WithMany(x => x.BrowsingHistory)
                .HasForeignKey(x => x.UserId);
        }
    }
}
