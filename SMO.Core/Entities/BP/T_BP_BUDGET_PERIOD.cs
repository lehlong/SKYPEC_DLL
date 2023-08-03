using System;
using System.Collections.Generic;

namespace SMO.Core.Entities.BP
{
    public class T_BP_BUDGET_PERIOD : BaseEntity
    {
        public T_BP_BUDGET_PERIOD()
        {
            History = new List<T_BP_BUDGET_PERIOD_HISTORY>();
        }
        public virtual string ID { get; set; }
        /// <summary>
        /// Mã giai đoạn
        /// </summary>
        public virtual int PERIOD_ID { get; set; }
        /// <summary>
        /// Năm ngân sách
        /// </summary>
        public virtual int TIME_YEAR { get; set; }
        /// <summary>
        /// Trạng thái: Đóng mở giai đoạn
        /// </summary>
        public virtual bool STATUS { get; set; }
        /// <summary>
        /// Có tự động chuyển giai đoạn hay không
        /// </summary>
        public virtual bool AUTO_NEXT_PERIOD { get; set; }
        /// <summary>
        /// Có thông báo cho người dùng khi chuyển giai đoạn hay không
        /// </summary>
        public virtual bool NOTIFY_USER { get; set; }
        /// <summary>
        /// Thời gian chuyển giai đoạn
        /// </summary>
        public virtual DateTime? TIME_NEXT_PERIOD { get; set; }
        public virtual T_BP_PERIOD Period { get; set; }
        public virtual IList<T_BP_BUDGET_PERIOD_HISTORY> History { get; set; }
    }
}
