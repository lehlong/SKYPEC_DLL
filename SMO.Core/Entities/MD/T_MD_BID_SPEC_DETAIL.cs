namespace SMO.Core.Entities
{
    public partial class T_MD_BID_SPEC_DETAIL : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string HEADER_ID { get; set; }
        public virtual decimal? FROM_VALUE { get; set; }
        public virtual decimal? TO_VALUE { get; set; }
        public virtual string VALUE { get; set; }
        public virtual string VALUE_EN { get; set; }
        public virtual int SCORE { get; set; }
        public virtual int C_ORDER { get; set; }
    }
}
