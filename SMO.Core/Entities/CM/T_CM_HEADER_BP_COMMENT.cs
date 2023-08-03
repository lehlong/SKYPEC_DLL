using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.Core.Entities.CM
{
    public class T_CM_HEADER_BP_COMMENT : BaseEntity
    {
        public T_CM_HEADER_BP_COMMENT()
        {
            Comments = new List<T_CM_COMMENT>();
        }
        public virtual string PKID { get; set; }
        /// <summary>
        /// Code của center
        /// </summary>
        public virtual string ORG_CODE { get; set; }
        /// <summary>
        /// Tùy thuộc vào comment mà reference code chứa code của element code hoặc code của template code
        /// </summary>
        public virtual string REFERENCE_CODE { get; set; }
        public virtual int YEAR { get; set; }
        public virtual int VERSION { get; set; }
        public virtual int NUMBER_COMMENTS { get; set; }
        /// <summary>
        /// Lấy dữ liệu từ SMO.AppCode.Class.TemplateObjectType
        /// </summary>
        public virtual string OBJECT_TYPE { get; set; }
        /// <summary>
        /// Lấy dữ liệu từ SMO.BudgetType class
        /// </summary>
        public virtual string BUDGET_TYPE { get; set; }
        /// <summary>
        /// Lấy dữ liệu từ SMO.ElementType
        /// </summary>
        public virtual string ELEMENT_TYPE { get; set; }

        public virtual string CONTENT { get; set; }

        public virtual T_MD_COST_CENTER CostCenter { get; set; }
        public virtual IList<T_CM_COMMENT> Comments { get; set; }
    }
}
