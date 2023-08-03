using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SMO.Repository.Common
{
    public interface IGenericRepository<T>
    {
        DateTime GetDateDatabase();
        int ExecuteUpdate(string sql);
        IQueryable<T> Queryable();
        T Create(T obj);
        IEnumerable<T> Create(IEnumerable<T> lstObj);
        T CreateOrUpdate(T obj);
        T Update(T obj);
        void Delete(T obj);
        void Delete(List<T> lstObj);
        void Delete(object id);
        void Delete(List<object> lstId);
        T Load(object id);
        T Get(object id, dynamic param = null);
        IList<T> GetByProperty(string property, object value);
        IList<T> GetByHQL(string hql);
        IList<T> GetAll();
        IList<T> GetListActive();
        IList<T> GetAllOrdered(string propertyName, bool Ascending = true);
        IList<T> Find(IList<string> criteria);
        void Detach(T item);
        IList<T> Paging(IQueryable<T> query, int pageSize, int pageIndex, out int total);
        IList<T> Search(T objFilter, int pageSize, int pageIndex, out int total);
        bool CheckExist(Func<T, bool> predicate);
        IList<T> GetManyByExpression(Func<T, bool> prediction);
        T GetFirstByExpression(Func<T, bool> prediction);
        T GetNewestByExpression<Tkey>(Func<T, bool> prediction, Func<T, Tkey> order, bool isDescending);
        void Delete(Func<T, bool> prediction);
        IList<T> GetManyWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch);
        T GetFirstWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch);
        void Update(Func<T, bool> filter, params Action<T>[] actions);
        IList<T> GetManyWithFetchMany(Expression<Func<T, bool>> prediction, params Expression<Func<T, IEnumerable<object>>>[] fetch);
    }
}
