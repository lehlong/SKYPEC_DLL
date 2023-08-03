using System.Collections.Generic;

namespace SMO.Core.Entities.WF
{
    public partial class T_WF_ACTIVITY : BaseEntity
    {
        public T_WF_ACTIVITY()
        {
            ActivityUsers = new List<T_WF_ACTIVITY_USER>();
            ActivityComs = new List<T_WF_ACTIVITY_COM>();
        }
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string PROCESS_CODE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual T_WF_PROCESS Process { get; set; }

        public virtual IList<T_WF_ACTIVITY_USER> ActivityUsers { get; set; }
        public virtual IList<T_WF_ACTIVITY_COM> ActivityComs { get; set; }

    }
}
