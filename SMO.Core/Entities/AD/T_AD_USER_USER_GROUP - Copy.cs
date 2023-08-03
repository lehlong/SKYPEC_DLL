using System;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_USER_GROUP : BaseEntity
    {
        public virtual string USER_NAME { get; set; }
        public virtual string USER_GROUP_CODE { get; set; }
        public virtual T_AD_USER User { get; set; }
        public virtual T_AD_USER_GROUP UserGroup { get; set; }

        public T_AD_USER_USER_GROUP()
        {
            User = new T_AD_USER();
            UserGroup = new T_AD_USER_GROUP();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is T_AD_USER_USER_GROUP other)) return false;
            return ReferenceEquals(this, other) ? true : USER_NAME == other.USER_NAME && USER_GROUP_CODE == other.USER_GROUP_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                hash = (hash * 31) ^ USER_GROUP_CODE.GetHashCode();
                return hash;
            }
        }
    }
}
