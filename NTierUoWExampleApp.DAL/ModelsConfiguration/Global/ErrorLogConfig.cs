using NTierUoWExampleApp.DAL.Models.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.ModelsConfiguration.Global
{
    public class ErrorLogConfig : EntityTypeConfiguration<ErrorLog>
    {
        public ErrorLogConfig()
        {
            HasKey(p => p.ErrorLogId);
            Property(p => p.ErrorLogId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
