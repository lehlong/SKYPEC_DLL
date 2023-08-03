using System;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_ROLE : BaseEntity
    {
        public virtual string ROLE_CODE { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual T_AD_ROLE Role { get; set; }
        public virtual T_AD_USER User { get; set; }

        public T_AD_USER_ROLE()
        {
            Role = new T_AD_ROLE();
            User = new T_AD_USER();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is T_AD_USER_ROLE other)) return false;
            return ReferenceEquals(this, other) ? true : ROLE_CODE == other.ROLE_CODE && USER_NAME == other.USER_NAME;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ ROLE_CODE.GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                return hash;
            }
        }
    }
}
