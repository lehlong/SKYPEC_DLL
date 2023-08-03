using System;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_RIGHT : BaseEntity
    {
        public virtual string USER_NAME { get; set; }
        public virtual string FK_RIGHT { get; set; }
        public virtual string ORG_CODE { get; set; }
        public virtual bool IS_ADD { get; set; }
        public virtual bool IS_REMOVE { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is T_AD_USER_RIGHT other)) return false;
            return ReferenceEquals(this, other) ? true : USER_NAME == other.USER_NAME && FK_RIGHT == other.FK_RIGHT && ORG_CODE == other.ORG_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                hash = (hash * 31) ^ FK_RIGHT.GetHashCode();
                hash = (hash * 31) ^ ORG_CODE.GetHashCode();
                return hash;
            }
        }
    }
}
