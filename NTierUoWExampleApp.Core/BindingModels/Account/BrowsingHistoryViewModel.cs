using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.Account
{
    public class BrowsingHistoryViewModel
    {
        public BrowsingHistoryViewModel()
        {

        }
        public BrowsingHistoryViewModel(BrowsingHistory history)
        {
            DateTimeUtc = history.DateTimeUtc.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            PageUrl = history.PageUrl;
            UserAgent = history.UserAgent;
        }

        public string DateTimeUtc { get; set; }
        public string DateTimeServer { get; set; }
        public string PageUrl { get; set; }
        public string UserAgent { get; set; }
    }
}
