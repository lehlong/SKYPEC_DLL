namespace SMO.Core.Entities
{
    public partial class T_MD_PERIOD_TIME : BaseEntity
    {
        public virtual int TIME_YEAR { get; set; }
        public virtual bool IS_CLOSE { get; set; }
        public virtual bool IS_EDIT { get; set; }
        public virtual bool IS_DEFAULT { get; set; }
    }
}
