using SharpSapRfc;

namespace SMO.Core.Entities.MD
{
    public class T_SAP_MD_PAYMENT : BaseEntity
    {
        [RfcStructureField("XREF1")]
        public virtual string CODE { get; set; }
        [RfcStructureField("XREF1_HD")]
        public virtual string PARENT_CODE { get; set; }
        [RfcStructureField("Z_VALUE")]
        public virtual string NAME { get; set; }
        [RfcStructureField("VALUE_HD")]
        public virtual string GROUP_NAME { get; set; }
        public virtual int C_ORDER { get; set; }

    }
}
