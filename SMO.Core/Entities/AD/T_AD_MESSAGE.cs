namespace SMO.Core.Entities
{
    public partial class T_AD_MESSAGE : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string CODE { get; set; }
        public virtual string LANGUAGE { get; set; }
        public virtual string MESSAGE { get; set; }
    }
}
