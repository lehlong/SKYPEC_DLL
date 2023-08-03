namespace SMO.Core.Entities
{
    public partial class T_CM_COMMENT : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string REFRENCE_ID { get; set; }
        public virtual string CONTENTS { get; set; }
    }
}
