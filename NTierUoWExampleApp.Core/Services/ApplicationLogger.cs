using NTierUoWExampleApp.DAL.Models.Global;
using NTierUoWExampleApp.DAL.UnitOfWork.IUoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Services
{
    internal class ApplicationLogger
    {
        public ApplicationLogger(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private IUnitOfWork unitOfWork = null;

        public async Task LogError(string errorMethodName, string logType, string errorMessage, string InnerExceptionMessage, string stackTrace)
        {
            ErrorLog error = new ErrorLog()
            {
                DateTimeUtc = DateTime.UtcNow,
                ErrorMessage = errorMessage,
                ErrorMethodName = errorMethodName,
                DateTimeServer = DateTime.Now,
                InnerExceptionMessage = InnerExceptionMessage,
                LogType = logType,
                StackTrace = stackTrace
            };

            unitOfWork.ErrorLogRepository.Create(error);
            await unitOfWork.SaveChangesAsync();

        }

    }
}
