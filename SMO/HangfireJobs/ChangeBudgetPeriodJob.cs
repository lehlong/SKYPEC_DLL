using SMO.AppCode.Status;
using SMO.Core.Entities.BP;
using SMO.Repository.Implement.BP;
using SMO.Service.BP;

using System;

namespace SMO.HangfireJobs
{
    public class ChangeBudgetPeriodJob
    {
        /// <summary>
        /// chạy 1 lần hàng ngày để xem có giai đoạn nào cần chuyển hay không
        /// </summary>
        public static void AutoChangePeriod()
        {
            var budgetPeriodService = new BudgetPeriodService();
            var unitOfWork = budgetPeriodService.UnitOfWork;
            var currentDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            // lấy tất cả danh sách có tự động chuyển giai đoạn và ngày chuyển là ngày hiện tại
            var autoChangePeriod = budgetPeriodService.GetManyWithFetch(x => x.AUTO_NEXT_PERIOD
            && (!x.TIME_NEXT_PERIOD.HasValue || (x.TIME_NEXT_PERIOD.Value <= currentDate && x.TIME_NEXT_PERIOD.Value >= DateTime.Now.Date)));

            try
            {
                unitOfWork.BeginTransaction();
                foreach (var budgetPeriod in autoChangePeriod)
                {
                    var shouldNotifyUsers = false;
                    var isChangeStatus = false;
                    if (!budgetPeriod.STATUS)
                    {
                        // update close or open period
                        unitOfWork.Repository<BudgetPeriodHistoryRepo>().Create(new T_BP_BUDGET_PERIOD_HISTORY
                        {
                            ID = Guid.NewGuid().ToString(),
                            ACTION_DATE = DateTime.Now,
                            ACTION_USER = "Tự động",
                            BUDGET_PERIOD_ID = budgetPeriod.ID,
                            TYPE = BudgetPeriodAction.CHUYEN_GIAI_DOAN_MO
                        });

                        if (budgetPeriod.TIME_NEXT_PERIOD.HasValue)
                        {
                            // active this period
                            budgetPeriod.STATUS = true;
                            isChangeStatus = true;
                            shouldNotifyUsers = true && budgetPeriod.NOTIFY_USER;
                        }
                        else
                        {
                            // find earliest budget period with next time value in the same period
                            var nearliestBudgetPeriod = budgetPeriodService.GetNewestByExpression(x => x.AUTO_NEXT_PERIOD && x.TIME_NEXT_PERIOD.HasValue && x.PERIOD_ID == budgetPeriod.PERIOD_ID && x.TIME_YEAR < budgetPeriod.TIME_YEAR, order: x => x.TIME_YEAR, isDescending: true);
                            if (nearliestBudgetPeriod.TIME_NEXT_PERIOD.Value.AddYears(budgetPeriod.TIME_YEAR - nearliestBudgetPeriod.TIME_YEAR) <= currentDate)
                            {
                                // active this period
                                budgetPeriod.STATUS = true;
                                isChangeStatus = true;
                                shouldNotifyUsers = true && budgetPeriod.NOTIFY_USER;
                            }
                        }
                    }
                    if (shouldNotifyUsers && budgetPeriod.NOTIFY_USER)
                    {
                        NotifyUtilities.CreateNotifyChangePeriod(budgetPeriod.Period, budgetPeriod.TIME_YEAR);
                    }
                    if(isChangeStatus)
                    {
                        // disable previous status
                        // get previous status
                        var previousPeriod = unitOfWork.Repository<BudgetPeriodRepo>().GetFirstWithFetch(x => x.TIME_YEAR == budgetPeriod.TIME_YEAR && x.Period.NEXT_PERIOD_ID == budgetPeriod.PERIOD_ID);
                        if (previousPeriod.STATUS)
                        {
                            // evict entity
                            unitOfWork.Repository<BudgetPeriodRepo>().Detach(previousPeriod);
                            previousPeriod.STATUS = false;

                            // add history
                            unitOfWork.Repository<BudgetPeriodHistoryRepo>().Create(new T_BP_BUDGET_PERIOD_HISTORY
                            {
                                ID = Guid.NewGuid().ToString(),
                                ACTION_DATE = DateTime.Now,
                                ACTION_USER = "Tự động",
                                BUDGET_PERIOD_ID = previousPeriod.ID,
                                TYPE = BudgetPeriodAction.CHUYEN_GIAI_DOAN_DONG
                            });
                        }
                    }
                }
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                // notify to admin
            }
        }
    }
}
