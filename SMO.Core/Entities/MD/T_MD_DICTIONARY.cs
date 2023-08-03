namespace SMO.Core.Entities
{
    public class T_MD_DICTIONARY : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string FK_DOMAIN { get; set; }
        public virtual string CODE { get; set; }
        public virtual string LANG { get; set; }
        public virtual string C_VALUE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual bool C_DEFAULT { get; set; }
    }
}
