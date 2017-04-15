using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Account
{
    public class UserNotification
    {
        public Guid UserNotificationId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string Message { get; set; }
        public DateTime TimeUtc { get; set; }
        public bool IsSeen { get; set; } //indicates if the user has read the message
        public string Type { get; set; }
    }
}
