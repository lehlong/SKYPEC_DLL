using SMO.Core.Entities.MD;

using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.BP
{
    public abstract class T_BP_BASE : BaseEntity
    {
        public virtual string PKID { get; set; }
        [Display(Name = "Bộ phận")]
        public virtual string ORG_CODE { get; set; }
        [Display(Name = "Mẫu")]
        public virtual string TEMPLATE_CODE { get; set; }
        [Display(Name = "Version")]
        public virtual int VERSION { get; set; }
        [Display(Name = "Năm")]
        public virtual int TIME_YEAR { get; set; }
        public virtual string STATUS { get; set; }
        public virtual string PHASE { get; set; }
        public virtual string FILE_ID { get; set; }
        public virtual bool IS_SUMUP { get; set; }
        public virtual bool IS_REVIEWED { get; set; }
        public virtual bool IS_DELETED { get; set; }
        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }
        public virtual T_CM_FILE_UPLOAD FileUpload { get; set; }

        public virtual bool? StatusFilter { get; set; }     // use for filter in view. Not storage in db

    }
}
