using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Notification.Email.Config
{
    public class Message : IdentityMessage
    {
        public Message()
        {
            Attachments = new List<KeyValuePair<string, Stream>>();
        }
        public string From { get; set; }
        public List<KeyValuePair<string, Stream>> Attachments { get; set; }
    }
}
