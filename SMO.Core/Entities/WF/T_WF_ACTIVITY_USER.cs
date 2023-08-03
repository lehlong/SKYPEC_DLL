using SMO.Core.Entities.MD;

namespace SMO.Core.Entities.WF
{
    public class T_WF_ACTIVITY_USER : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string USER_SENDER { get; set; }
        public virtual string USER_RECEIVER { get; set; }
        public virtual string ACTIVITY_CODE { get; set; }
        public virtual string ORG_CODE { get; set; }

        public virtual T_AD_USER UserSender { get; set; }
        public virtual T_AD_USER UserReceiver { get; set; }
        public virtual T_WF_ACTIVITY Activity { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }

    }
}
