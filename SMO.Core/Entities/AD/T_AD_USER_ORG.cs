using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_ORG : BaseEntity
    {
        public virtual string ORG_CODE { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }
        public virtual T_AD_USER User { get; set; }

        public T_AD_USER_ORG()
        {
            Organize = new T_MD_COST_CENTER();
            User = new T_AD_USER();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is T_AD_USER_ORG other)) return false;
            return ReferenceEquals(this, other) ? true : ORG_CODE == other.ORG_CODE && USER_NAME == other.USER_NAME;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ ORG_CODE.GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                return hash;
            }
        }
    }
}
