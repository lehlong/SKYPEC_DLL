using SMO.Core.Entities.MD;

namespace SMO.Core.Entities.BP.COST_CF
{
    public class T_BP_CONTRUCT_COST_CF_REVIEW_RESULT : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string HEADER_ID { get; set; }
        public virtual string COST_CF_ELEMENT_CODE { get; set; }
        public virtual bool? RESULT { get; set; }
        public virtual int TIME_YEAR { get; set; }

        public virtual T_MD_COST_CF_ELEMENT Element { get; set; }
        public virtual T_BP_CONTRUCT_COST_CF_REVIEW Header { get; set; }
    }
}
