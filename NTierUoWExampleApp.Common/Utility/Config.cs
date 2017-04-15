using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Common.Utility
{
    public class Config
    {
        public static string ConnectionStringName
        {
            get
            {
                if (ConfigurationManager.AppSettings["ConnectionStringName"] != null)
                {
                    return ConfigurationManager.AppSettings["ConnectionStringName"].ToString();
                }
                return "";
            }
        }

        public static string AppURL
        {
            get
            {
                if (ConfigurationManager.AppSettings["AppURL"] != null)
                {
                    return ConfigurationManager.AppSettings["AppURL"].ToString();
                }
                return "";
            }
        }

        public static bool UserLockoutEnabledByDefault
        {
            get
            {
                if (ConfigurationManager.AppSettings["UserLockoutEnabledByDefault"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["UserLockoutEnabledByDefault"].ToString());
                }
                return true;
            }
        }


        public static double DefaultAccountLockoutTimeSpan
        {
            get
            {
                if (ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"] != null)
                {
                    return Double.Parse(ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString());
                }
                return 15; //min
            }
        }



        public static int MaxFailedAccessAttemptsBeforeLockout
        {
            get
            {
                if (ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"] != null)
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"].ToString());
                }
                return 5; //attempts
            }
        }
    }
}
