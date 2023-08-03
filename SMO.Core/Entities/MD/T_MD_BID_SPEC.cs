using System.Collections.Generic;

namespace SMO.Core.Entities
{
    public partial class T_MD_BID_SPEC : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string PARENT_ID { get; set; }
        public virtual string NAME { get; set; }
        public virtual string NAME_EN { get; set; }
        public virtual string DESCRIPTION { get; set; }
        public virtual string DESCRIPTION_EN { get; set; }
        public virtual string TYPE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual bool IS_SPEC_PRICE { get; set; }
        public virtual bool IS_NOT_SHOW_CONTRACTOR { get; set; }
        public virtual bool IS_NOT_COPY { get; set; }
        public virtual string ROLE { get; set; }
        public virtual string VALUE_OF_SPEC { get; set; }
        public virtual IList<T_MD_BID_SPEC_DETAIL> ListDetail { get; set; }

        public T_MD_BID_SPEC()
        {
            ListDetail = new List<T_MD_BID_SPEC_DETAIL>();
        }
    }
}
