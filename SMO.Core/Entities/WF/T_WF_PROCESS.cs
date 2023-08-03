using System.Collections.Generic;

namespace SMO.Core.Entities.WF
{
    public partial class T_WF_PROCESS : BaseEntity
    {
        public T_WF_PROCESS()
        {
            Activities = new List<T_WF_ACTIVITY>();
        }
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }

        public virtual IList<T_WF_ACTIVITY> Activities { get; set; }
    }
}
