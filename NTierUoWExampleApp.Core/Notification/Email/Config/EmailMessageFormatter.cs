using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Notification.Email.Config
{
    public class EmailMessageFormatter
    {
        public EmailMessageFormatter()
        {

        }

        public EmailMessageFormatter(MessageConfig config)
        {
            this.config = config;
        }

        const string ResourcePathTemplate = "NTierUoWExampleApp.Core.Notification.Email.Templates.HTML.{0}.html";
        private MessageConfig config = null;

        string LoadTemplate(string name)
        {
            name = string.Format(ResourcePathTemplate, name);


            var asm = typeof(EmailMessageFormatter).Assembly;
            using (var s = asm.GetManifestResourceStream(name))
            {
                if (s == null) return null;
                using (var sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public Message Format()
        {
            Message msg = null;
            if (this.config == null)
                throw new ArgumentException("MessageConfig object not initialized");
            msg = this.CreateMessage(GetSubject(config.MailType.ToString()), GetBody(config.MailType.ToString()));
            return msg;

        }

        protected virtual string GetSubject(string mailType)
        {
            return FormatValue(LoadSubjectTemplate(mailType));
        }
        protected virtual string GetBody(string mailType)
        {
            return FormatValue(LoadBodyTemplate(mailType));
        }
        protected virtual string LoadSubjectTemplate(string templateName)
        {
            return LoadTemplate(templateName + "_Subject");
        }
        protected virtual string LoadBodyTemplate(string templateName)
        {
            return LoadTemplate(templateName + "_Body");
        }

        protected string FormatValue(string msg)
        {
            if (msg == null) return null;

            msg = msg.Replace("{username}", config.UserName);
            msg = msg.Replace("{email}", config.Email);


            msg = msg.Replace("{applicationName}", config.ApplicationName);
            msg = msg.Replace("{emailSignature}", config.EmailSignature);
            msg = msg.Replace("{loginUrl}", config.LoginUrl);

            msg = msg.Replace("{confirmAccountCreateUrl}", config.VerifyAccountUrl);
            msg = msg.Replace("{cancelNewAccountUrl}", config.CancelNewAccountUrl);

            msg = msg.Replace("{confirmPasswordResetUrl}", config.ConfirmPasswordResetUrl);

            return msg;
        }

        protected Message CreateMessage(string subject, string body)
        {
            if (subject == null || body == null) return null;

            return new Message { Subject = subject, Body = body };
        }
    }
}
