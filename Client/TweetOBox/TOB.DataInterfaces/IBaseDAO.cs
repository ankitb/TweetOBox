using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace TOB.DataInterfaces
{
    public interface IBaseDAO
    {
        TEntity Get<TEntity>(Expression<Func<TEntity, bool>> wherePredicate) where TEntity : class;
        List<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> wherePredicate) where TEntity : class;
        List<TEntity> GetListBySorting<TEntity, TKey>(Expression<Func<TEntity, bool>> wherePredicate,
            Expression<Func<TEntity, TKey>> keySelector, SortOrder sortOrder) where TEntity : class;

        List<TEntity> GetLimitedListBySorting<TEntity, TKey>(Expression<Func<TEntity, bool>> wherePredicate,
        Expression<Func<TEntity, TKey>> keySelector, SortOrder sortOrder, int count) where TEntity : class;

        List<TEntity> GetAll<TEntity>() where TEntity : class;
        List<TEntity> GetAllBySorting<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector, SortOrder sortOrder) where TEntity : class;
        void SaveChanges();
        void Insert<TEntity>(TEntity item) where TEntity : class;
        void InsertAll<TEntity>(IEnumerable<TEntity> list) where TEntity : class;
        void Attach<TEntity>(TEntity item) where TEntity : class;
        void Attach<TEntity>(TEntity item, TEntity original) where TEntity : class;
        void AttachAll<TEntity>(IEnumerable<TEntity> list) where TEntity : class;
        void Delete<TEntity>(TEntity item) where TEntity : class;
        void DeleteAll<TEntity>(IEnumerable<TEntity> list) where TEntity : class;
        TEntity Rollback<TEntity>(TEntity item) where TEntity : class;
        void Refresh();
        
    }
}
