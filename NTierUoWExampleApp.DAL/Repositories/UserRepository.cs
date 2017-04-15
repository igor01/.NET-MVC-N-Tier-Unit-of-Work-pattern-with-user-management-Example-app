using NTierUoWExampleApp.DAL.DBInitialization;
using NTierUoWExampleApp.DAL.Models.Account;
using NTierUoWExampleApp.DAL.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Repositories
{
    internal class UserRepository : GenericRepository<User>, IUserRepository
    {
        private ApplicationContext context;
        public UserRepository(ApplicationContext context)
            : base(context)
        {
            this.context = context;
        }

        public User FindByEmail(string email)
        {
            return DbSet.FirstOrDefault(x => x.Email == email);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return DbSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task<User> FindByEmailAsync(CancellationToken cancellationToken, string email)
        {
            return DbSet.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public User FindByUserName(string username)
        {
            return DbSet.FirstOrDefault(x => x.UserName == username);
        }

        public Task<User> FindByUserNameAsync(string username)
        {
            return DbSet.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public Task<User> FindByUserNameAsync(CancellationToken cancellationToken, string username)
        {
            return DbSet.FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);
        }
    }
}
