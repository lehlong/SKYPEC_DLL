namespace SMO.Core.Entities.WF
{
    public class T_WF_ACTIVITY_COM : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string ACTIVITY_CODE { get; set; }
        public virtual string TYPE_NOTIFY { get; set; }
        public virtual string SUBJECT { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual T_WF_ACTIVITY Activity { get; set; }
    }
}
