using SMO.Core.Entities;
using SMO.Repository.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SMO.Service
{
    public abstract class GenericService<T, TRepo> : BaseService, IGenericService<T> where T : BaseEntity where TRepo : GenericRepository<T>
    {
        public T ObjDetail { get; set; }
        public List<T> ObjList { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IGenericRepository<T> CurrentRepository { get; set; }

        public GenericService()
        {
            ObjDetail = Activator.CreateInstance(typeof(T)) as T;
            ObjList = new List<T>();
            UnitOfWork = new NHUnitOfWork();
            CurrentRepository = UnitOfWork.Repository<TRepo>();
        }

        public virtual string GetSequence(string modulType)
        {
            var code = DateTime.Now.ToString("yyMMdd");
            var sequence = UnitOfWork.GetSession().CreateSQLQuery("exec GET_SEQUENCE :ModulType")
                .SetParameter("ModulType", modulType)
                .UniqueResult();
            int iResult = Int32.Parse(sequence.ToString());
            code += string.Format("{0:0}", iResult);
            if (modulType == "MATERIAL")
            {
                return iResult.ToString();
            }

            if (modulType == "TEMPLATE")
            {
                return iResult.ToString();
            }
            return code;
        }

        public virtual void Search()
        {
            try
            {
                //UnitOfWork.BeginTransaction();

                ObjList = CurrentRepository.Search(ObjDetail, NumerRecordPerPage, Page, out int iTotalRecord).ToList();
                TotalRecord = iTotalRecord;
                //UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                //UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }


        public virtual void Get(object id, dynamic param = null)
        {
            try
            {
                //UnitOfWork.BeginTransaction();

                ObjDetail = this.CurrentRepository.Get(id, param);

                //UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                //UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }


        public virtual void GetAll()
        {
            try
            {
                //UnitOfWork.BeginTransaction();

                ObjList = CurrentRepository.GetAll().ToList();

                //UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                //UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public virtual void Create()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                if (ProfileUtilities.User != null)
                {
                    ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                    //this.ObjDetail.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                }

                ObjDetail = CurrentRepository.Create(ObjDetail);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public virtual void Update()
        {
            try
            {
                UnitOfWork.BeginTransaction();

                ObjDetail.UPDATE_BY = ProfileUtilities.User?.USER_NAME;

                ObjDetail = CurrentRepository.Update(ObjDetail);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }
        public virtual void Update(Func<T, bool> filter, params Action<T>[] actions)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                actions.Add(new Action<T>(x => x.UPDATE_BY = ProfileUtilities.User?.USER_NAME));

                CurrentRepository.Update(filter, actions);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public virtual void Delete(List<object> lstId)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                CurrentRepository.Delete(lstId);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public virtual void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                Delete(lstId);
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }

        }

        public virtual bool CheckExist(Func<T, bool> predicate)
        {
            try
            {
                //UnitOfWork.BeginTransaction();
                var result = CurrentRepository.CheckExist(predicate);
                //UnitOfWork.Commit();
                return result;
            }
            catch
            {
                //UnitOfWork.Rollback();
                return false;
            }
        }

        public virtual void ToggleActive(object id)
        {
            try
            {
                Get(id);
                UnitOfWork.BeginTransaction();

                if (ProfileUtilities.User != null)
                {
                    ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    //this.ObjDetail.UPDATE_DATE = this.CurrentRepository.GetDateDatabase();
                }

                ObjDetail.ACTIVE = !ObjDetail.ACTIVE;
                ObjDetail = CurrentRepository.Update(ObjDetail);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public virtual bool GetState()
        {
            return State;
        }

        public virtual T GetFirstByExpression(Func<T, bool> prediction)
        {
            return CurrentRepository.GetFirstByExpression(prediction);
        }

        public virtual T GetNewestByExpression<Tkey>(Func<T, bool> prediction, Func<T, Tkey> order, bool isDescending)
        {
            return CurrentRepository.GetNewestByExpression(prediction, order, isDescending);
        }

        public virtual IList<T> GetManyByExpression(Func<T, bool> prediction)
        {
            return CurrentRepository.GetManyByExpression(prediction);
        }

        public virtual IList<T> GetManyWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch)
        {
            return CurrentRepository.GetManyWithFetch(prediction, fetch);
        }
        public virtual T GetFirstWithFetch(Expression<Func<T, bool>> prediction, params Expression<Func<T, object>>[] fetch)
        {
            return CurrentRepository.GetFirstWithFetch(prediction, fetch);
        }

        public virtual IList<IEntity> GetAllMasterData<IRepo, IEntity>() where IRepo : GenericRepository<IEntity> where IEntity : BaseEntity
        {
            return UnitOfWork.Repository<IRepo>().GetAll();
        }

    }
}
