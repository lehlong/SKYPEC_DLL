using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace SMO.Core.Entities.MD
{
    public class T_MD_TEMPLATE : BaseEntity, ICloneable
    {
        public T_MD_TEMPLATE()
        {
            DetailCosts = new List<T_MD_TEMPLATE_DETAIL_COST_PL>();
            DetailRevenues = new List<T_MD_TEMPLATE_DETAIL_REVENUE_PL>();
        }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [MaxLength(length: 50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string NAME { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string TITLE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string ORG_CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string ELEMENT_TYPE { get; set; }
        /// <summary>
        /// Lấy giá trị từ constant class BudgetType
        /// </summary>
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string BUDGET_TYPE { get; set; }
        /// <summary>
        /// Lấy giá trị từ constant class TemplateObjectType
        /// </summary>
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string OBJECT_TYPE { get; set; }
        public virtual string NOTES { get; set; }
        public virtual bool IS_BASE { get; set; }

        [ScriptIgnore]
        [JsonIgnore]
        public virtual T_MD_COST_CENTER Organize { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_COST_PL> DetailCosts { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_COST_CF> DetailCostsCF { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL> DetailContructCostPL { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF> DetailContructCostCF { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_OTHER_COST_PL> DetailOtherCostPL { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_OTHER_COST_CF> DetailOtherCostCF { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> DetailRevenues { get; set; }
        [ScriptIgnore]
        [JsonIgnore]
        public virtual IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> DetailRevenuesCF { get; set; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
