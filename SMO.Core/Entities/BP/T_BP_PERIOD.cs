namespace SMO.Core.Entities.BP
{
    public class T_BP_PERIOD : BaseEntity
    {
        public virtual int ID { get; set; }
        public virtual string NAME { get; set; }
        public virtual int ORDER { get; set; }
        public virtual int NEXT_PERIOD_ID { get; set; }
        public virtual string NOTES { get; set; }
    }
}
