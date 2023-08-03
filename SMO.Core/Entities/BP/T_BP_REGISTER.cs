using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.BP
{
    public class T_BP_REGISTER : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual int TIME_YEAR { get; set; }
        public virtual string ORG_CODE { get; set; }
        public virtual string DESCRIPTION { get; set; }
        public virtual Guid TYPE_ID { get; set; }
        public virtual bool IS_REGISTER { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }
        public virtual T_BP_TYPE BpType { get; set; }
    }
}
