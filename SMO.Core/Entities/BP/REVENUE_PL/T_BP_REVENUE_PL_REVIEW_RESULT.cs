using SMO.Core.Entities.MD;

namespace SMO.Core.Entities.BP.REVENUE_PL
{
    public class T_BP_REVENUE_PL_REVIEW_RESULT : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string HEADER_ID { get; set; }
        public virtual string REVENUE_PL_ELEMENT_CODE { get; set; }
        public virtual bool? RESULT { get; set; }
        public virtual int TIME_YEAR { get; set; }

        public virtual T_MD_REVENUE_PL_ELEMENT Element { get; set; }
        public virtual T_BP_REVENUE_PL_REVIEW Header { get; set; }
    }
}
