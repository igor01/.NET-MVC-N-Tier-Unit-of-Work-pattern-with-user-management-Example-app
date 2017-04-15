using Microsoft.AspNet.Identity;
using NTierUoWExampleApp.Core.Notification.Email.Config;
using NTierUoWExampleApp.Core.Notification.Email.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Notification.Email
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            Gmail mail = new Gmail();
            return mail.SendAsync(message, null, null);
        }

        public Task SendAsync(Message message)
        {
            Gmail mail = new Gmail();
            return mail.SendAsync(message, null, message.Attachments);
        }

        public static Message MessageFromTemplate(MessageConfig config)
        {
            var msgFormater = new EmailMessageFormatter(config);
            Message message = msgFormater.Format();
            return message;
        }
    }
}
