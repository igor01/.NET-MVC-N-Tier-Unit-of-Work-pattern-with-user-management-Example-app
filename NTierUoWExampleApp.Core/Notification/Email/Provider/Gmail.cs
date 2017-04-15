using Microsoft.AspNet.Identity;
using NTierUoWExampleApp.Core.Notification.Email.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Notification.Email.Provider
{
    public class Gmail
    {
        public Gmail()
        {

        }

        public Gmail(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public async Task SendAsync(IdentityMessage msg, string from, IEnumerable<KeyValuePair<string, Stream>> attachments)
        {
            try
            {
                SmtpSection smtp = null;
                if (String.IsNullOrWhiteSpace(from))
                {
                    smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                    from = smtp.From;
                }

                MailMessage mail = new MailMessage();
                mail.To.Add(msg.Destination);
                mail.From = new MailAddress(from);
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.Body = msg.Body;
                mail.Subject = msg.Subject;
                mail.IsBodyHtml = true;

                if (attachments != null && attachments.Count() > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        attachment.Value.Position = 0;
                        mail.Attachments.Add(new Attachment(attachment.Value, attachment.Key));
                    }

                }

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = smtp.Network.DefaultCredentials;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Port = smtp.Network.Port;
                smtpClient.EnableSsl = smtp.Network.EnableSsl;
                smtpClient.Credentials = new System.Net.NetworkCredential(smtp.Network.UserName, smtp.Network.Password);

                await smtpClient.SendMailAsync(mail);

            }
            catch (Exception exe)
            {

                throw exe;
            }
        }

        public async Task SendAsync(Message msg)
        {
            try
            {
                SmtpSection smtp = null;
                if (String.IsNullOrWhiteSpace(msg.From))
                {
                    smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                    msg.From = smtp.From;
                }

                MailMessage mail = new MailMessage();
                mail.To.Add(msg.Destination);
                mail.From = new MailAddress(msg.From);
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.Body = msg.Body;
                mail.Subject = msg.Subject;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = smtp.Network.DefaultCredentials;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Port = smtp.Network.Port;
                smtpClient.EnableSsl = smtp.Network.EnableSsl;
                smtpClient.Credentials = new System.Net.NetworkCredential(smtp.Network.UserName, smtp.Network.Password);

                await smtpClient.SendMailAsync(mail);
            }
            catch (Exception exe)
            {

                throw exe;
            }
        }
        private void MailDeliveryComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception("Error sending email");
            }
            else if (e.Cancelled)
            {
                throw new Exception("Sending of email cancelled.");
            }
            else
            {
                throw new Exception("Message sent");
            }

        }
        public void Send(Message msg)
        {
            try
            {
                SmtpSection smtp = null;

                if (String.IsNullOrWhiteSpace(msg.From))
                {
                    smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                    msg.From = smtp.From;
                }

                using (SmtpClient smtpClient = new SmtpClient())
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(msg.Destination);
                    mail.From = new MailAddress(msg.From);
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.Body = msg.Body;
                    mail.Subject = msg.Subject;
                    mail.IsBodyHtml = true;

                    smtpClient.UseDefaultCredentials = smtp.Network.DefaultCredentials;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Port = smtp.Network.Port;
                    smtpClient.EnableSsl = smtp.Network.EnableSsl;

                    smtpClient.Credentials = new System.Net.NetworkCredential(smtp.Network.UserName, smtp.Network.Password);

                    smtpClient.Send(mail);
                }
            }
            catch (Exception exe)
            {

                throw exe;
            }


        }
    }
}
