namespace SMO.Core.Entities.MD
{
    public class T_MD_USER_REVIEW : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual int TIME_YEAR { get; set; }

        public virtual T_AD_USER User { get; set; }
    }
}
