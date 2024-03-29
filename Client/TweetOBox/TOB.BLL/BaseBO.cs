﻿using System;
using System.Collections.Generic;
using TOB.DataInterfaces;
using TOB.DAL;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace TOB.BLL
{
    /// <summary>
    /// Base class for all the BOs in an application. Any BO is generated by passing the entity type of the class. This class uses the Data Access 
    /// Factory to create base dao.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseBO<TEntity>
        where TEntity : class
    {
        protected IDAOFactory _daoFactory;
        protected IBaseDAO _dao;

        public BaseBO()
        {
            _daoFactory = new TOBLinqTOBFactory();
            _dao = _daoFactory.CreateBaseDAO();
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> wherePredicate)
        {
            return _dao.Get<TEntity>(wherePredicate);
        }

        public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> wherePredicate)
        {
            return _dao.GetList<TEntity>(wherePredicate);
        }

        public virtual List<TEntity> GetListBySorting<TKey>(Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, TKey>> keySelector, SortOrder sortOrder)
        {
            return _dao.GetListBySorting<TEntity, TKey>(wherePredicate, keySelector, sortOrder);
        }

        public virtual List<TEntity> GetLimitedListBySorting<TKey>(Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, TKey>> keySelector, SortOrder sortOrder, int count)
        {
            return _dao.GetLimitedListBySorting<TEntity, TKey>(wherePredicate, keySelector, sortOrder, count);
        }


        public virtual List<TEntity> GetAll()
        {
            return _dao.GetAll<TEntity>();
        }

        public virtual List<TEntity> GetAllBySorting<TKey>(Expression<Func<TEntity, TKey>> keySelector, System.Data.SqlClient.SortOrder sortOrder)
        {
            return _dao.GetAllBySorting<TEntity, TKey>(keySelector, sortOrder);
        }

        public virtual void Insert(TEntity item)
        {
            _dao.Insert<TEntity>(item);
        }

        public virtual void SynchronizeCollection<TKey>(IEnumerable<TEntity> modifiedList, Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, TKey>> keySelector)
            where TKey : struct
        {
            //Attach those entities where primary key is not equal to 0
            if (modifiedList != null)
            {
                var attachList = (from a in modifiedList
                                  where keySelector.Compile().DynamicInvoke(a).ToString() != "0"
                                  select a).ToList();
                AttachAll(attachList);

                //Delete those entities that does not exist in our collection
                var originalList = GetList(wherePredicate);
                var deleteList = new List<TEntity>();
                foreach (var item in originalList)
                {
                    IEnumerable<TEntity> query;
                    query = from a in modifiedList
                            where keySelector.Compile().DynamicInvoke(a).ToString() == keySelector.Compile().DynamicInvoke(item).ToString()
                            select a;

                    if (query.FirstOrDefault() == null)
                        deleteList.Add(item);
                }

                DeleteAll(deleteList);
            }
        }

        public virtual void InsertAll(IEnumerable<TEntity> list)
        {
            _dao.InsertAll<TEntity>(list);
        }

        public virtual void Attach(TEntity item)
        {
            _dao.Attach<TEntity>(item);
        }

        public virtual void Attach(TEntity item, TEntity original)
        {
            _dao.Attach<TEntity>(item, original);
        }

        public virtual void AttachAll(IEnumerable<TEntity> modifiedList)
        {
            _dao.AttachAll<TEntity>(modifiedList);
        }

        public virtual int GetIdentityColumnValue(TEntity item, string identityColumnName)
        {
            Type type = typeof(TEntity);
            PropertyInfo propertyInfo = type.GetProperty(identityColumnName);
            return (int)propertyInfo.GetValue(item, null);
        }

        public virtual bool IsEqual(TEntity item, TEntity other, string identityColumnName)
        {
            if (GetIdentityColumnValue(item, identityColumnName) == GetIdentityColumnValue(other, identityColumnName))
                return true;
            else
                return false;

        }

        public virtual void SaveChanges()
        {
            _dao.SaveChanges();
            return;
        }

        public virtual void Delete(TEntity item)
        {
            _dao.Delete<TEntity>(item);
        }

        public virtual void DeleteAll(IEnumerable<TEntity> list)
        {
            _dao.DeleteAll<TEntity>(list);
        }

        public virtual TEntity Rollback(TEntity item)
        {
            return _dao.Rollback<TEntity>(item);
        }

        public void Refresh()
        {
            _dao.Refresh();
        }
    }
}
