using System;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_ROLE_DETAIL : BaseEntity
    {
        public virtual string FK_ROLE { get; set; }
        public virtual string FK_RIGHT { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is T_AD_ROLE_DETAIL other)) return false;
            return ReferenceEquals(this, other) ? true : FK_ROLE == other.FK_ROLE && FK_RIGHT == other.FK_RIGHT;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ FK_ROLE.GetHashCode();
                hash = (hash * 31) ^ FK_RIGHT.GetHashCode();
                return hash;
            }
        }
    }
}
