using System;

namespace SMO.Core.Entities.BP
{
    public class T_BP_BUDGET_PERIOD_HISTORY : BaseEntity
    {
        public virtual string ID { get; set; }
        public virtual string BUDGET_PERIOD_ID { get; set; }
        public virtual string TYPE { get; set; }
        public virtual string ACTION_USER { get; set; }
        public virtual DateTime ACTION_DATE { get; set; }

        public virtual T_BP_BUDGET_PERIOD BudgetPeriod { get; set; }
    }
}
