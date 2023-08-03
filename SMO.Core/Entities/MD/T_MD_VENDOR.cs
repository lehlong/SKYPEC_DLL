using SharpSapRfc;

namespace SMO.Core.Entities
{
    public partial class T_MD_VENDOR : BaseEntity
    {
        [RfcStructureField("LIFNR")]
        public virtual string CODE { get; set; }
        [RfcStructureField("NAME1")]
        public virtual string SHORT_NAME { get; set; }
        [RfcStructureField("CNAME")]
        public virtual string LONG_NAME { get; set; }
        [RfcStructureField("STCEG")]
        public virtual string MA_SO_THUE { get; set; }
        [RfcStructureField("ADD")]
        public virtual string DIA_CHI { get; set; }
        [RfcStructureField("TELF1")]
        public virtual string SO_DIEN_THOAI { get; set; }
        [RfcStructureField("TELFX")]
        public virtual string SO_FAX { get; set; }
        [RfcStructureField("EMAIL")]
        public virtual string EMAIL { get; set; }
        [RfcStructureField("URL")]
        public virtual string WEBSITE { get; set; }
        [RfcStructureField("CONTA")]
        public virtual string LIEN_HE { get; set; }
        [RfcStructureField("MAJBS")]
        public virtual string LINH_VUC_CHINH { get; set; }
        [RfcStructureField("EXTENSION1")]
        public virtual string KHU_VUC { get; set; }
        [RfcStructureField("REGIO")]
        public virtual string TINH_THANH { get; set; }
        [RfcStructureField("DVTGD")]
        public virtual string DON_VI_TUNG_GIAO_DICH { get; set; }
        public virtual bool STATUS { get; set; }
        public virtual string USER_NAME { get; set; }
    }
}
