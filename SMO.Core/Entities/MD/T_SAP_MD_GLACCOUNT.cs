using SharpSapRfc;

namespace SMO.Core.Entities.MD
{
    public class T_SAP_MD_GLACCOUNT : BaseEntity
    {
        [RfcStructureField("SAKNR")]
        public virtual string CODE { get; set; }
        [RfcStructureField("KTOKS")]
        public virtual string PARENT_CODE { get; set; }
        [RfcStructureField("TXT50")]
        public virtual string NAME { get; set; }
        [RfcStructureField("TXT30")]
        public virtual string GROUP_NAME { get; set; }
        public virtual int C_ORDER { get; set; }

    }
}
