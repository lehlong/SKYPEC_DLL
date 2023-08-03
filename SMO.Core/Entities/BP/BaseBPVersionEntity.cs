using SMO.Core.Entities.MD;

namespace SMO.Core.Entities.BP
{
    public class BaseBPVersionEntity : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string ORG_CODE { get; set; }
        public virtual string TEMPLATE_CODE { get; set; }
        public virtual int VERSION { get; set; }
        public virtual int TIME_YEAR { get; set; }
        public virtual string FILE_ID { get; set; }
        public virtual int IS_DELETED { get; set; }
        public virtual T_CM_FILE_UPLOAD FileUpload { get; set; }

        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }
    }
}
