using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Account
{
    public class BrowsingHistory
    {
        public Guid BrowsingHistoryId { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime DateTimeUtc { get; set; }
        public DateTime DateTimeServer { get; set; }
        public string PageUrl { get; set; }
        public string UserAgent { get; set; }
    }
}
