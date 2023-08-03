namespace SMO.Core.Entities
{
    public partial class T_CF_TEMPLATE_NOTIFY : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string INVITE_SUBJECT { get; set; }
        public virtual string INVITE_BODY { get; set; }
        public virtual string INVITE_COUNCIL_SUBJECT { get; set; }
        public virtual string INVITE_COUNCIL_BODY { get; set; }
        public virtual string SCORE_SUBJECT { get; set; }
        public virtual string SCORE_BODY { get; set; }
        public virtual string WIN_SUBJECT { get; set; }
        public virtual string WIN_BODY { get; set; }
        public virtual string FAIL_SUBJECT { get; set; }
        public virtual string FAIL_BODY { get; set; }
        public virtual string ACCOUNT_SUBJECT { get; set; }
        public virtual string ACCOUNT_BODY { get; set; }
        public virtual string REQUEST_ACCOUNT_SUBJECT { get; set; }
        public virtual string REQUEST_ACCOUNT_BODY { get; set; }

        public virtual string CONG_VIEC_XU_LY_SUBJECT { get; set; }
        public virtual string CONG_VIEC_XU_LY_BODY { get; set; }
        public virtual string CONG_VIEC_HOAN_THANH_SUBJECT { get; set; }
        public virtual string CONG_VIEC_HOAN_THANH_BODY { get; set; }
        public virtual string FEED_BACK_SUBJECT { get; set; }
        public virtual string FEED_BACK_BODY { get; set; }

        public virtual string UPDATE_INFO_BID_SUBJECT { get; set; }
        public virtual string UPDATE_INFO_BID_BODY { get; set; }
    }
}
