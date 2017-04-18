using NTierUoWExampleApp.DAL.Models.Account;
using NTierUoWExampleApp.DAL.Models.Authentication;
using NTierUoWExampleApp.DAL.Models.Global;
using NTierUoWExampleApp.DAL.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.UnitOfWork.IUoW
{
    public interface IUnitOfWork : IDisposable
    {

        //bulk insert
        void BulkInsert(string connectionString, string destinationTableName, DataTable dataTable, List<string> dataTableProperties);

        //stored procedure
        Task<int> ExecuteStoredProcedureAsync(string storedProcedureName);
        Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, SqlParameter[] parameters);


        //Users
        IUserRepository UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<UserRole> UserRoleRepository { get; }
        IRepository<UserWebClientConnection> UserWebClientConnectionRepository { get; }
        IRepository<BrowsingHistory> BrowsingHistoryRepository { get; }


        //Web API
        IRepository<Client> ClientRepository { get; }
        IRepository<RefreshToken> RefreshTokenRepository { get; }
        IRepository<ClientUsers> ClientUsersRepository { get; }


        //Logging
        IRepository<ErrorLog> ErrorLogRepository { get; }


        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
