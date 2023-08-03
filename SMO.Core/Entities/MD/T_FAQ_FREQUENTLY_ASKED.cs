namespace SMO.Core.Entities
{
    public partial class T_FAQ_FREQUENTLY_ASKED : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string TIEU_DE { get; set; }
        public virtual string TIEU_DE_EN { get; set; }
        public virtual string NOI_DUNG { get; set; }
        public virtual string NOI_DUNG_EN { get; set; }
        public virtual int C_ORDER { get; set; }
    }
}
