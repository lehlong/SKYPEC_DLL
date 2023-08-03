using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Repository.Implement.BP;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.BP
{
    public class BaseBPReviewService<TEntity, TElement, TRepo> : GenericService<TEntity, TRepo> where TEntity: BPBaseReviewEntity<TElement> where TRepo : GenericRepository<TEntity>
    {
        /// <summary>
        /// Có phải là hội đồng thẩm định hay không
        /// </summary>
        public bool IsReview { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsNotCompleted { get; set; }
        public bool? Status { get; set; }

        internal virtual bool ValidateBudgetPeriod(int year, BudgetPeriod budgetPeriod)
        {
            var bp = UnitOfWork.Repository<BudgetPeriodRepo>().GetFirstWithFetch(x => x.TIME_YEAR == year && x.PERIOD_ID == (int)budgetPeriod, x => x.Period);
            if (bp == null)
            {
                ErrorMessage = $"Thông tin cấu hình giai đoạn cho năm {year} chưa được cấu hình.";
                State = false;
                return false;
            }
            else if (!bp.STATUS)
            {
                ErrorMessage = $"Giai đoạn: {bp.Period.NAME} của năm {year} đã đóng và không thể chỉnh sửa.";
                State = false;
                return false;
            }
            else
            {
                return true;
            }
        }

        internal TEntity GetDetail()
        {
            if (IsReview)
            {
                return GetFirstByExpression(x =>
                x.ORG_CODE.Equals(ObjDetail.ORG_CODE) &&
                x.DATA_VERSION == ObjDetail.DATA_VERSION &&
                x.TIME_YEAR == ObjDetail.TIME_YEAR &&
                x.REVIEW_USER == ProfileUtilities.User.USER_NAME &&
                !x.IS_SUMMARY);
            }
            else
            {
                return GetFirstByExpression(x =>
                x.ORG_CODE.Equals(ObjDetail.ORG_CODE) &&
                x.DATA_VERSION == ObjDetail.DATA_VERSION &&
                x.TIME_YEAR == ObjDetail.TIME_YEAR &&
                x.IS_SUMMARY);
            }
        }
    }
}
