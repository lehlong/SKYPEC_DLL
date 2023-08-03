using SMO.AppCode.Status;
using SMO.Core.Entities.BP;
using SMO.Repository.Implement.BP;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.BP
{
    public class BudgetPeriodService : GenericService<T_BP_BUDGET_PERIOD, BudgetPeriodRepo>
    {
        public BudgetPeriodService():base()
        {
            History = new List<T_BP_BUDGET_PERIOD_HISTORY>();
        }

        public IList<T_BP_BUDGET_PERIOD_HISTORY> History { get; set; }
        public override void Search()
        {
            State = true;

            base.Search();

            if (ObjList.Count == 0)
            {
                // first time go to budget period view
                // create for this year
                // get all period
                var periods = UnitOfWork.Repository<PeriodRepo>().GetAll();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var currentDate = DateTime.Now;
                ObjList = (from period in periods.OrderBy(x => x.ORDER)
                           select new T_BP_BUDGET_PERIOD
                           {
                               ID = Guid.NewGuid().ToString(),
                               Period = period,
                               PERIOD_ID = period.ID,
                               TIME_YEAR = ObjDetail.TIME_YEAR,
                               CREATE_BY = currentUser,
                               CREATE_DATE = currentDate,
                               STATUS = false
                           }).ToList();
                try
                {
                    UnitOfWork.BeginTransaction();
                    CurrentRepository.Create(ObjList);
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

        internal void GetHistory(string id)
        {
            History = UnitOfWork.Repository<BudgetPeriodHistoryRepo>().GetAllHistory(id);
        }

        public override void Update()
        {
            // kiểm tra xem nếu có tự động chuyển giai đoạn hay k 
            // nếu có, check xem có để lấy ngày từ năm trước của giai đoạn này hay không (trường ngày bằng null)
            // nếu có thì check xem những năm trước có để giá trị trường này hay chưa
            // nếu không báo lỗi
            // nếu không thì lưu dữ liệu
            var currentUser = ProfileUtilities.User?.USER_NAME;
            ObjDetail.UPDATE_BY = currentUser;
            var shouldUpdate = false;
            // lấy dữ liệu hiện tại
            var budgetPeriodCurrentInDb = CurrentRepository.Get(ObjDetail.ID);
            if (ObjDetail.AUTO_NEXT_PERIOD)
            {
                if (ObjDetail.TIME_NEXT_PERIOD.HasValue)
                {
                    shouldUpdate = true;
                }
                else
                {
                    // kiểm tra những năm trước có giá trị time next và auto next chưa
                    var previousYearOfThisPeriod = GetFirstWithFetch(x => x.TIME_YEAR < ObjDetail.TIME_YEAR && x.AUTO_NEXT_PERIOD && x.TIME_NEXT_PERIOD.HasValue && x.PERIOD_ID == ObjDetail.PERIOD_ID);
                    if (previousYearOfThisPeriod == null)
                    {
                        State = false;
                        ErrorMessage = "Không thể kế thừa thời gian tự động chuyển giai đoạn do các năm trước chưa được cấu hình thời gian tự động chuyển giai đoạn của giai đoạn này.";
                        return;
                    }
                    else
                    {
                        shouldUpdate = true;
                    }
                }
            }
            else
            {
                // tìm năm phía sau có để chế độ auto next và thừa kế time từ trước không
                var afterYearOfThisPeriod = GetNewestByExpression(prediction: x => x.TIME_YEAR > ObjDetail.TIME_YEAR && x.AUTO_NEXT_PERIOD && x.PERIOD_ID == ObjDetail.PERIOD_ID, order: x => x.TIME_YEAR, isDescending: false);
                if (afterYearOfThisPeriod == null || afterYearOfThisPeriod.TIME_NEXT_PERIOD.HasValue)
                {
                    shouldUpdate = true;
                }
                else
                {
                    // tìm những năm phía trước có năm nào để chế độ auto next không
                    var previouYearOfThisPeriod = GetNewestByExpression(prediction: x => x.TIME_YEAR < ObjDetail.TIME_YEAR && x.AUTO_NEXT_PERIOD && x.PERIOD_ID == ObjDetail.PERIOD_ID, order: x => x.TIME_YEAR, isDescending: false);
                    if (previouYearOfThisPeriod != null)
                    {
                        shouldUpdate = true;
                    }
                    else
                    {
                        State = false;
                        ErrorMessage = "Không thể tắt chế độ tự động chuyển giai đoạn này do đang tồn tại năm phụ thuộc.";
                        return;
                    }
                }
            }
            if (shouldUpdate)
            {
                try
                {
                    UnitOfWork.BeginTransaction();
                    if (budgetPeriodCurrentInDb.STATUS != ObjDetail.STATUS)
                    {
                        // update close or open period
                        UnitOfWork.Repository<BudgetPeriodHistoryRepo>().Create(new T_BP_BUDGET_PERIOD_HISTORY
                        {
                            ID = Guid.NewGuid().ToString(),
                            ACTION_DATE = DateTime.Now,
                            ACTION_USER = currentUser,
                            BUDGET_PERIOD_ID = ObjDetail.ID,
                            TYPE = ObjDetail.STATUS ? BudgetPeriodAction.MO_GIAI_DOAN : BudgetPeriodAction.DONG_GIAI_DOAN
                        });
                    }
                    // detach 
                    CurrentRepository.Detach(budgetPeriodCurrentInDb);
                    CurrentRepository.Update(ObjDetail);
                    UnitOfWork.Commit();
                    if (ObjDetail.NOTIFY_USER && !budgetPeriodCurrentInDb.STATUS && ObjDetail.STATUS)
                    {
                        // notify all user
                        NotifyUtilities.CreateNotifyChangePeriod(budgetPeriodCurrentInDb.Period, ObjDetail.TIME_YEAR);
                    }

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
}
