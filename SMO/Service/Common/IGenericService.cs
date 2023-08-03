using SMO.Core.Entities;
using SMO.Repository.Common;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SMO.Service
{
    public interface IGenericService<T>
    {
        void Create();
        void Update();
        void Delete(List<object> lstId);
        void Delete(string strLstSelected);
        void Get(object id, dynamic param = null);
        void GetAll();
        void Search();
        bool CheckExist(Func<T, bool> predicate);
        void ToggleActive(object id);
        bool GetState();
        IList<T> GetManyByExpression(Func<T, bool> prediction);
        T GetFirstByExpression(Func<T, bool> prediction);
        T GetNewestByExpression<Tkey>(Func<T, bool> prediction, Func<T, Tkey> order, bool isDescending);
        T GetFirstWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch);
        IList<T> GetManyWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch);
        IList<IEntity> GetAllMasterData<IRepo, IEntity>()
            where IRepo : GenericRepository<IEntity>
            where IEntity : BaseEntity;
    }
}
