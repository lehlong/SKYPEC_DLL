using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_GROUP : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string NOTES { get; set; }
        public virtual IList<T_AD_USER_USER_GROUP> ListUserUserGroup { get; set; }
        public virtual IList<T_AD_USER_GROUP_ROLE> ListUserGroupRole { get; set; }

        public T_AD_USER_GROUP()
        {
            ListUserUserGroup = new List<T_AD_USER_USER_GROUP>();
            ListUserGroupRole = new List<T_AD_USER_GROUP_ROLE>();
        }
    }
}
