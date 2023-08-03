namespace SMO.Core.Entities.MD
{
    public class T_SAP_MD_COST_PL_ELEMENT : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string PARENT_CODE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual string NAME { get; set; }
    }
}
