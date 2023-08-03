namespace SMO.Core.Entities
{
    public partial class T_FAQ_QUESTION : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string NAME { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string SUBJECT { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual string ANSWER { get; set; }
        public virtual bool STATUS { get; set; }
    }
}
