using NTierUoWExampleApp.DAL.DBInitialization;
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
    internal class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private ApplicationContext context;
        private DbSet<TEntity> dbSet;

        internal GenericRepository(ApplicationContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        protected DbSet<TEntity> DbSet
        {
            get { return dbSet ?? (dbSet = context.Set<TEntity>()); }
        }


        public List<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return DbSet.ToListAsync();
        }

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return DbSet.ToListAsync(cancellationToken);
        }



        public IQueryable<TEntity> GetAllQueryable()
        {
            return DbSet;
        }



        public List<TEntity> PageAll(int skip, int take)
        {
            return DbSet.Skip(skip).Take(take).ToList();
        }

        public Task<List<TEntity>> PageAllAsync(int skip, int take)
        {
            return DbSet.Skip(skip).Take(take).ToListAsync();
        }

        public Task<List<TEntity>> PageAllAsync(CancellationToken cancellationToken, int skip, int take)
        {
            return DbSet.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }



        public TEntity FindById(object id)
        {
            return DbSet.Find(id);
        }

        public Task<TEntity> FindByIdAsync(object id)
        {
            return DbSet.FindAsync(id);
        }

        public Task<TEntity> FindByIdAsync(CancellationToken cancellationToken, object id)
        {
            return DbSet.FindAsync(cancellationToken, id);
        }




        public void Create(TEntity entity)
        {
            if (this.context.Entry(entity).State != EntityState.Detached)
            {
                this.context.Entry(entity).State = EntityState.Added;
            }
            else
            {
                this.dbSet.Add(entity);
            }

        }

        public void CreateRange(List<TEntity> entityList)
        {
            this.dbSet.AddRange(entityList);
        }


        public void Update(TEntity entity)
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
                entry = context.Entry(entity);
            }
            entry.State = EntityState.Modified;
        }
        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }



        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
