using System;
using System.Linq;
using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore;

namespace LeeVox.Demo.BigBank.Data
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected internal IBigBankDbContext CustoMerDbContext {get; set;}

        public BaseRepository(IBigBankDbContext custoMerDbContext)
        {
            this.CustoMerDbContext = custoMerDbContext;
        }

        public IQueryable<TEntity> All
        {
            get
            {
                return CustoMerDbContext.GetDbSet<TEntity>().Where(e => !e.__Deleted.HasValue);
            }
        }

        public IQueryable<TEntity> All_IncludeDeleted
        {
            get
            {
                return CustoMerDbContext.GetDbSet<TEntity>();
            }
        }

        public TEntity ById(int id)
        {
            return All.FirstOrDefault(e => e.Id == id);
        }

        public void Create(TEntity entity)
        {
            entity.__Created = DateTime.UtcNow;
            CustoMerDbContext.GetDbSet<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            entity.__Updated = DateTime.UtcNow;
            CustoMerDbContext.AttachEntity(entity).State = EntityState.Modified;
        }

        public void Delete(int id)
            => Delete(ById(id));

        public void Delete(TEntity entity)
        {
            // do not delete the entity from database
            entity.__Deleted = DateTime.UtcNow;
            CustoMerDbContext.AttachEntity(entity).State = EntityState.Modified;
        }
    }
}
