using SharpSapRfc;

namespace SMO.Core.Entities.MD
{
    public class T_SAP_MD_EXPENSE : BaseEntity
    {
        [RfcStructureField("ZKEY")]
        public virtual string CODE { get; set; }
        [RfcStructureField("ZKEY_HD")]
        public virtual string PARENT_CODE { get; set; }
        [RfcStructureField("ZVALUE")]
        public virtual string NAME { get; set; }
        [RfcStructureField("ZHD")]
        public virtual string GROUP_NAME { get; set; }
        public virtual int C_ORDER { get; set; }

    }
}
