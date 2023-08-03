using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    public class T_MD_PREVENTIVE : BaseEntity
    {
        public virtual string ID { get; set; }
        [Display(Name = "Phòng ban")]
        public virtual string ORG_CODE { get; set; }
        [Display(Name = "Ghi chú")]
        public virtual string DESCRIPTION { get; set; }
        [Display(Name = "Năm kế hoạch")]
        public virtual int TIME_YEAR { get; set; }
        [Display(Name = "Kế hoạch dự phòng")]
        [DisplayFormat(DataFormatString = @"{0:#\%}", ApplyFormatInEditMode = true)]
        public virtual decimal PERCENTAGE { get; set; }

        public virtual T_MD_COST_CENTER CostCenter { get; set; }
    }
}
