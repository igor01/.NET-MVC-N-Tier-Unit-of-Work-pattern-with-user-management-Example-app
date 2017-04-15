using NTierUoWExampleApp.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Configuration
{
    public class ApplicationInformation
    {
        public ApplicationInformation()
        {
            Host = Config.AppURL;
            ApplicationName = "Demo";
            EmailSignature = "Demo Team";
            VerifyAccountUrl = Host + "/User/Register";
            CancelNewAccountUrl = Host + "/User/Cancel";
            LoginUrl = Host + "/User/Login";
            ConfirmPasswordResetUrl = Host + "/User/ResetPassword";

        }


        public virtual string Host { get; set; }
        public virtual string ApplicationName { get; set; }
        public virtual string EmailSignature { get; set; }
        public virtual string LoginUrl { get; set; }
        public virtual string VerifyAccountUrl { get; set; }
        public virtual string CancelNewAccountUrl { get; set; }
        public virtual string ConfirmPasswordResetUrl { get; set; }
    }
}
