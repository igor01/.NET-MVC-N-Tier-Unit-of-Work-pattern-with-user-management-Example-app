using NTierUoWExampleApp.DAL.DBInitialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Configuration
{
    public class InitDatabase
    {
        public void Init()
        {
            ApplicationStart applicationStart = new ApplicationStart();
            applicationStart.InitializeDatabase();
        }
    }
}
