namespace SMO.Core.Entities
{
    public partial class T_CM_HISTORY_SMO_API : BaseEntity
    {
        public virtual int ID { get; set; }
        public virtual string C_FUNCTION { get; set; }
        public virtual string PARAMETER { get; set; }
        public virtual string RESULT { get; set; }
    }
}
