using SMO.Core.Entities;
using SMO.Repository.Implement.MD;

using System;

namespace SMO.Service.MD
{
    public class PeriodTimeService : GenericService<T_MD_PERIOD_TIME, PeriodTimeRepo>
    {
        public PeriodTimeService() : base()
        {

        }

        public override void Create()
        {
            try
            {
                if (!CheckExist(x => x.TIME_YEAR == ObjDetail.TIME_YEAR))
                {
                    this.ObjDetail.IS_CLOSE = false;
                    this.ObjDetail.IS_EDIT = false;
                    base.Create();
                }
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }
        }

        internal void SetDefaultTimeYear(int year)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                CurrentRepository.Update(x => x.IS_DEFAULT, x => x.IS_DEFAULT = false);
                CurrentRepository.Update(x => x.TIME_YEAR == year, x => x.IS_DEFAULT = true);
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = e;
            }
        }

        internal void ToggleStatus(int year)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                CurrentRepository.Update(x => x.TIME_YEAR == year, x => x.IS_CLOSE = !x.IS_CLOSE);
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = e;
            }
        }
    }
}
