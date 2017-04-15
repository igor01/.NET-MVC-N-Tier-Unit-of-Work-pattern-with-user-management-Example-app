using NTierUoWExampleApp.Common.Enum.Email;
using NTierUoWExampleApp.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Notification.Email.Config
{
    public class MessageConfig : ApplicationInformation
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }


        public EmailTypeEnum MailType { get; set; }


        // URL in body of mail 
        public override string VerifyAccountUrl
        {
            get
            {
                string url = string.Format(base.VerifyAccountUrl + "?userId={0}&code={1}", new[] { UserId, Token });
                return url;
            }
        }
        public override string CancelNewAccountUrl
        {
            get
            {
                string url = string.Format(base.CancelNewAccountUrl + "?code={0}", new[] { Token });
                return url;
            }
        }
        public override string ConfirmPasswordResetUrl
        {
            get
            {
                string url = string.Format(base.ConfirmPasswordResetUrl + "?code={0}", new[] { Token });
                return url;
            }
        }
    }
}
