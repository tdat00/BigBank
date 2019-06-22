using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Data
{
    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        IQueryable<TEntity> All {get;}
        IQueryable<TEntity> IncludeProperty<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
            where TProperty : class;
        
        TEntity ById(int id);

        void Create(TEntity entity);
        void Create(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(int id);
        void Delete(TEntity entity);
    }
}
