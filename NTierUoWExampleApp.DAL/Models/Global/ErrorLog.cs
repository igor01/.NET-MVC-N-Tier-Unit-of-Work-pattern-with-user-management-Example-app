using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Global
{
    public class ErrorLog
    {
        public Guid ErrorLogId { get; set; }
        public string LogType { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public DateTime DateTimeServer { get; set; }
        public string ErrorMethodName { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
