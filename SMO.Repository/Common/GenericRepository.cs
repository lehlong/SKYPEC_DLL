using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

using SMO.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SMO.Repository.Common
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        public Type persitentType = typeof(T);
        //private NHUnitOfWork _unitOfWork;

        public GenericRepository(ISession _session)
        {
            NHibernateSession = _session;
        }

        /// <summary>
        /// </summary>
        public ISession NHibernateSession { get; set; }

        public virtual DateTime GetDateDatabase()
        {
            try
            {
                //ORACLE
                //var dateNow = NHibernateSession.CreateSQLQuery("select sysdate from dual").UniqueResult();
                //MSSSQL
                var dateNow = NHibernateSession.CreateSQLQuery("SELECT GETDATE()").UniqueResult();
                if (dateNow != null)
                {
                    return (DateTime)dateNow;
                }
            }
            catch { }
            return DateTime.Now;
        }

        public virtual int ExecuteUpdate(string sql)
        {
            var query = NHibernateSession.CreateSQLQuery(sql);
            int result = query.ExecuteUpdate();
            return result;
        }

        public virtual IQueryable<T> Queryable()
        {
            return NHibernateSession.Query<T>();
        }

        public virtual IList<T> Search(T objFilter, int pageSize, int pageIndex, out int total)
        {
            total = 0;
            return new List<T>();
        }

        /// <summary>
        /// For entities that have assigned ID's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/reference/en/html/mapping.html#mapping-declaration-id-assigned.
        /// </summary>
        public virtual T Create(T obj)
        {
            NHibernateSession.Save(obj);
            return obj;
        }

        /// <summary>
        /// For entities that have assigned ID's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/reference/en/html/mapping.html#mapping-declaration-id-assigned.
        /// </summary>
        public virtual IEnumerable<T> Create(IEnumerable<T> lstObj)
        {
            NHibernateSession.SetBatchSize(100);
            foreach (var obj in lstObj)
            {
                NHibernateSession.Save(obj);
            }
            return lstObj;
        }

        /// <summary>
        /// For entities with automatatically generated IDs, such as identity, SaveOrUpdate may 
        /// be called when saving a new entity.  SaveOrUpdate can also be called to update any 
        /// entity, even if its ID is assigned.
        /// </summary>
        public virtual T CreateOrUpdate(T obj)
        {
            NHibernateSession.SaveOrUpdate(obj);
            return obj;
        }

        /// <summary>
        /// Update entity
        /// </summary>
        public virtual T Update(T obj)
        {
            Detach(obj);
            NHibernateSession.Update(obj);
            return obj;
        }

        public virtual void Delete(T obj)
        {
            NHibernateSession.Delete(obj);
        }

        public virtual void Delete(List<T> lstObj)
        {
            if (lstObj == null || lstObj.Count == 0)
            {
                return;
            }
            foreach (var item in lstObj)
            {
                Detach(item);
                NHibernateSession.Delete(item);
            }
        }

        public virtual void Delete(object id)
        {
            var obj = Get(id);
            if (obj != null)
            {
                Delete(obj);
            }
        }

        public virtual void Delete(List<object> listId)
        {
            foreach (var id in listId)
            {
                Delete(id);
            }
        }

        public virtual T Load(object id)
        {
            return NHibernateSession.Load<T>(id);
        }

        public virtual T Get(object id, dynamic param = null)
        {
            return NHibernateSession.Get<T>(id);
        }

        public virtual IList<T> GetByHQL(string hql)
        {
            var obj = NHibernateSession.CreateQuery(hql).List<T>();
            return obj;
        }

        public virtual IList<T> GetByProperty(string property, object value)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append(string.Format("FROM {0} a ", persitentType.FullName));
            hql.Append(string.Format("WHERE a.{0} = ?", property));
            var obj = NHibernateSession.CreateQuery(hql.ToString())
                .SetParameter(0, value)
                .List<T>();

            return obj;
        }

        public virtual IList<T> Paging(IQueryable<T> query, int pageSize, int pageIndex, out int total)
        {
            int startIndex = pageSize * (pageIndex - 1);
            var rowCount = query.ToFutureValue(x => x.Count());
            List<T> lstT = query.Skip(startIndex).Take(pageSize).ToFuture().ToList<T>();
            total = rowCount.Value;
            return lstT;
        }

        public virtual IList<T> Paging(IQueryOver<T> query, int pageSize, int pageIndex, out int total)
        {
            int startIndex = pageSize * (pageIndex - 1);
            var rowCount = query.ToRowCountQuery().FutureValue<int>();
            var lstT = query.Take(pageSize).Skip(startIndex).Future().ToList<T>();
            total = rowCount.Value;
            return lstT;
        }

        public virtual IList<T> GetAll()
        {
            var query = NHibernateSession.Query<T>();
            var result = query.ToList();
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<T>();
            }
        }

        public virtual IList<T> GetListActive()
        {
            var query = NHibernateSession.Query<T>();
            var result = query.Where(x => x.ACTIVE == true).ToList();
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<T>();
            }
        }

        public virtual IList<T> Find(IList<string> strs)
        {
            IList<ICriterion> objs = new List<ICriterion>();
            foreach (string s in strs)
            {
                ICriterion cr1 = NHibernate.Criterion.Expression.Sql(s);
                objs.Add(cr1);
            }
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            foreach (ICriterion rest in objs)
                NHibernateSession.CreateCriteria(persitentType).Add(rest);

            criteria.SetFirstResult(0);
            return criteria.List<T>();
        }

        public virtual void Detach(T obj)
        {
            NHibernateSession.Evict(obj);
        }

        public virtual IList<T> GetAllOrdered(string propertyName, bool ascending = true)
        {
            Order cr1 = new Order(propertyName, ascending);
            IList<T> objsResult = NHibernateSession.CreateCriteria(persitentType).AddOrder(cr1).List<T>();
            return objsResult;
        }

        public virtual bool CheckExist(Func<T, bool> predicate)
        {
            var query = NHibernateSession.Query<T>();
            if (query.Count(predicate) > 0)
            {
                NHibernateSession.Clear();
                return true;
            }
            NHibernateSession.Clear();
            return false;
        }

        public virtual T GetFirstByExpression(Func<T, bool> prediction)
        {
            var query = NHibernateSession.Query<T>();
            return query.Where(prediction).FirstOrDefault();
        }

        public virtual T GetNewestByExpression<Tkey>(Func<T, bool> prediction, Func<T, Tkey> order, bool isDescending)
        {
            if (prediction is null)
            {
                throw new ArgumentNullException(nameof(prediction));
            }

            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var query = NHibernateSession.Query<T>();
            if (isDescending)
            {
                return query.Where(prediction).OrderByDescending(order).FirstOrDefault();
            }
            else
            {
                return query.Where(prediction).OrderBy(order).FirstOrDefault();
            }
        }

        public virtual IList<T> GetManyByExpression(Func<T, bool> prediction)
        {
            var query = NHibernateSession.Query<T>();
            var queryOver = NHibernateSession.QueryOver<T>();

            return query.Where(prediction).ToList();
        }

        public virtual void Delete(Func<T, bool> prediction)
        {
            var lst = GetManyByExpression(prediction);
            foreach (var item in lst)
            {
                this.Detach(item);
            }
            Delete(lst.ToList());
        }

        /// <summary>
        /// Lấy danh sách item dựa vào filter và eager fetch thêm thuộc tính
        /// </summary>
        /// <param name="prediction">Expression muốn filter dữ liệu</param>
        /// <param name="fetch">Expression muốn fetch thuộc tính</param>
        /// <returns>Danh sách items</returns>
        public virtual IList<T> GetManyWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch)
        {
            var queryOver = NHibernateSession.Query<T>();
            queryOver = queryOver.Where(prediction);
            if (fetch != null && fetch.Count() > 0)
            {
                foreach (var f in fetch)
                {
                    queryOver = queryOver.Fetch(f);
                }
            }

            return queryOver.ToList();
        }

        /// <summary>
        /// Lấy danh sách item dựa vào filter và eager fetch thêm thuộc tính
        /// </summary>
        /// <param name="prediction">Expression muốn filter dữ liệu</param>
        /// <param name="fetch">Expression muốn fetch thuộc tính</param>
        /// <returns>Danh sách items</returns>
        public virtual IList<T> GetManyWithFetchMany(Expression<Func<T, bool>> prediction, params Expression<Func<T, IEnumerable<object>>>[] fetch)
        {
            var queryOver = NHibernateSession.Query<T>();
            queryOver = queryOver.Where(prediction);
            if (fetch != null && fetch.Count() > 0)
            {
                foreach (var f in fetch)
                {
                    queryOver = queryOver.FetchMany(f);
                }
            }

            return queryOver.ToList();
        }

        /// <summary>
        /// Lấy item đầu tiên cùng với eager fetch thuộc tính thỏa mãn filter
        /// </summary>
        /// <param name="prediction">Expression muốn filter dữ liệu</param>
        /// <param name="fetch">Expression muốn fetch thuộc tính</param>
        /// <returns>Item đầu tiên thỏa mãn</returns>
        public virtual T GetFirstWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch)
        {
            return GetManyWithFetch(prediction, fetch).FirstOrDefault();
        }

        /// <summary>
        /// Update many using filter
        /// </summary>
        /// <param name="filter">Expression to filter</param>
        /// <param name="actions">Action want to update</param>
        public void Update(Func<T, bool> filter, params Action<T>[] actions)
        {
            var lst = GetManyByExpression(filter);
            foreach (var item in lst)
            {
                foreach (var action in actions)
                {
                    action(item);
                }
                _ = Update(item);
            }
        }
    }
}
