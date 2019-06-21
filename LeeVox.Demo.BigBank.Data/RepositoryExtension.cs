using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace LeeVox.Demo.BigBank.Data
{
    public static class RepositoryExtension
    {
        public static IQueryable<TEntity> IncludeProperty<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> propertyPath)
            where TEntity : class
            => source.Include(propertyPath);
    }
}
