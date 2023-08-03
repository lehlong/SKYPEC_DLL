using System;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_APPROVE : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string USER_APPROVE { get; set; }
        public virtual string MODUL { get; set; }
    }
}
