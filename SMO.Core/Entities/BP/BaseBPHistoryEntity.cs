using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.BP
{
    public class BaseBPHistoryEntity : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string ORG_CODE { get; set; }
        public virtual string TEMPLATE_CODE { get; set; }
        public virtual int VERSION { get; set; }
        public virtual int TIME_YEAR { get; set; }
        public virtual string ACTION { get; set; }
        public virtual string ACTION_USER { get; set; }
        public virtual DateTime ACTION_DATE { get; set; }
        public virtual string NOTES { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }
        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_AD_USER ActionUser { get; set; }
    }
}
