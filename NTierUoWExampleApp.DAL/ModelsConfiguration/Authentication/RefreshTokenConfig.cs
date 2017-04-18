using NTierUoWExampleApp.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.ModelsConfiguration.Authentication
{
    public class RefreshTokenConfig : EntityTypeConfiguration<RefreshToken>
    {
        public RefreshTokenConfig()
        {
            HasKey(t => t.RefreshTokenId);

            HasRequired(t => t.Client)
                .WithMany(t => t.RefreshTokens)
                .HasForeignKey(t => t.ClientId);
        }
    }
}
