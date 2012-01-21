using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using TOB.DAL;
using TOB.DataInterfaces;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace TOB.DAL
{
    /// <summary>
    /// Abstract class for DAO. The purpose of this class is to initialize connection string 
    /// This functionality is common to each n every DAO classes. 
    /// So this has been made abstract base class. All DAO Classes should inherit this. Provided 
    /// they all dealing with country specific entities.
    /// </summary>
    public class BaseDAO : IBaseDAO, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected TOB _TOBDataContext;
        //private static System.IO.TextWriter _sqlTextWriter;
        private static bool _logInit = false;

        /// <summary>
        /// 
        /// </summary>        
        /// <summary>
        /// Baseclass constructor takes country id as the argument.
        /// It takes connection strings from the Central database country master table.
        /// </summary>
        /// <param name="countryid"></param>
        public BaseDAO()
        {
            _TOBDataContext = ContextFactory.CreateTOBDataContext();

            try
            {
                if (!_logInit)
                {
                    //_sqlTextWriter = new System.IO.StreamWriter("sqllog.txt");
                    _logInit = true;
                }
                //_TOBDataContext.Log = _sqlTextWriter;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            //_sqlTextWriter.Flush();
            _TOBDataContext.Dispose();
        }

        #endregion

        #region IBaseDAO Members

        public virtual TEntity Get<TEntity>(Expression<Func<TEntity, bool>> wherePredicate) where TEntity : class
        {
            return _TOBDataContext.GetTable<TEntity>().Where(wherePredicate).FirstOrDefault();
        }

        public List<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> wherePredicate) where TEntity : class
        {
            return _TOBDataContext.GetTable<TEntity>().Where(wherePredicate).ToList();
        }

        public List<TEntity> GetListBySorting<TEntity, TKey>(Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, TKey>> keySelector, SortOrder sortOrder) where TEntity : class
        {
            switch (sortOrder)
            {
                case SortOrder.Descending:
                    return _TOBDataContext.GetTable<TEntity>().Where(wherePredicate).OrderByDescending(keySelector).ToList();
                case SortOrder.Unspecified:
                default:
                    return _TOBDataContext.GetTable<TEntity>().Where(wherePredicate).OrderBy(keySelector).ToList();
            }
        }


        public List<TEntity> GetLimitedListBySorting<TEntity, Tkey>(Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, Tkey>> keySelector, SortOrder sortOrder, int count) where TEntity : class
        {
            switch (sortOrder)
            {
                case SortOrder.Descending:
                    return _TOBDataContext.GetTable<TEntity>().Where(wherePredicate).OrderByDescending(keySelector).Take(count).ToList();
                case SortOrder.Unspecified:
                default:
                    return _TOBDataContext.GetTable<TEntity>().Where(wherePredicate).OrderBy(keySelector).Take(count).ToList();
            }
        }

        public virtual List<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return _TOBDataContext.GetTable<TEntity>().ToList();
        }

        public virtual List<TEntity> GetAllBySorting<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector, SortOrder sortOrder) where TEntity : class
        {
            if (sortOrder == SortOrder.Ascending)
                return _TOBDataContext.GetTable<TEntity>().OrderBy(keySelector).ToList();
            else
                return _TOBDataContext.GetTable<TEntity>().OrderByDescending(keySelector).ToList();
        }


        public virtual void SaveChanges()
        {
            _TOBDataContext.SubmitChanges();
        }

        public virtual void Insert<TEntity>(TEntity item) where TEntity : class
        {
            _TOBDataContext.GetTable<TEntity>().InsertOnSubmit(item);
        }

        public virtual void InsertAll<TEntity>(IEnumerable<TEntity> list) where TEntity : class
        {
            _TOBDataContext.GetTable<TEntity>().InsertAllOnSubmit(list);
        }

        public virtual void Attach<TEntity>(TEntity item) where TEntity : class
        {
            _TOBDataContext.GetTable<TEntity>().Attach(item, true);
        }

        public virtual void Attach<TEntity>(TEntity item, TEntity original) where TEntity : class
        {
            _TOBDataContext.GetTable<TEntity>().Attach(item, original);
        }

        public virtual void AttachAll<TEntity>(IEnumerable<TEntity> list) where TEntity : class
        {
            _TOBDataContext.GetTable<TEntity>().AttachAll(list, true);
        }

        public virtual void Delete<TEntity>(TEntity item) where TEntity : class
        {
            _TOBDataContext.GetTable<TEntity>().DeleteOnSubmit(item);
        }

        public virtual void DeleteAll<TEntity>(IEnumerable<TEntity> list) where TEntity : class
        {
            _TOBDataContext.GetTable<TEntity>().DeleteAllOnSubmit(list);
        }

        public virtual TEntity Rollback<TEntity>(TEntity item) where TEntity : class
        {
            return _TOBDataContext.GetTable<TEntity>().GetOriginalEntityState(item);
        }

        public void Refresh()
        {
            _TOBDataContext.Refresh(RefreshMode.KeepChanges);
        }

        #endregion
    }
}
