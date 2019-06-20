using System;
using System.Collections.Generic;
using System.Linq;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected internal IBigBankDbContext DbContext {get; set;}
        private ILogger<IRepository<TEntity>> Logger { get; set; }

        public BaseRepository(IBigBankDbContext dbContext, ILogger<IRepository<TEntity>> logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }

        public IQueryable<TEntity> All
        {
            get
            {
                return DbContext.GetDbSet<TEntity>().Where(e => !e.__Deleted.HasValue);
            }
        }

        public IQueryable<TEntity> All_IncludeDeleted
        {
            get
            {
                return DbContext.GetDbSet<TEntity>();
            }
        }

        public TEntity ById(int id)
        {
            return All.FirstOrDefault(e => e.Id == id);
        }

        public void Create(TEntity entity)
        {
            entity.__Created = DateTime.UtcNow;
            DbContext.GetDbSet<TEntity>().Add(entity);
            Logger.LogDebug($"Create entity '{entity.GetType().Name}': {entity.ToJsonString()}.");
        }

        public void Create(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.__Created = DateTime.UtcNow;
            }
            DbContext.GetDbSet<TEntity>().AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            entity.__Updated = DateTime.UtcNow;
            DbContext.AttachEntity(entity).State = EntityState.Modified;
            Logger.LogDebug($"Updated entity '{entity.GetType().Name}': {entity.ToJsonString()}.");
        }

        public void Delete(int id)
            => Delete(ById(id));

        public void Delete(TEntity entity)
        {
            // do not delete the entity from database
            entity.__Deleted = DateTime.UtcNow;
            DbContext.AttachEntity(entity).State = EntityState.Modified;
            Logger.LogDebug($"Deleted entity '{entity.GetType().Name}': {entity.ToJsonString()}.");
        }
    }
}
