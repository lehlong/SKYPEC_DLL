using System.Collections.Generic;

namespace SMO.Models
{
    public class StepBudgetModel
    {
        public StepBudgetModel()
        {
            Steps = new List<StepBudgetItem>();
        }
        /// <summary>
        /// Năm ngân sách
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string CenterName { get; set; }
        /// <summary>
        /// Loại ngân sách
        /// </summary>
        public string BudgetName { get; set; }
        /// <summary>
        /// Các bước phê duyệt
        /// </summary>
        public IList<StepBudgetItem> Steps { get; set; }
    }
}
