using System;

namespace SMO.Core.Entities.BP
{
    public class T_BP_TYPE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string NAME { get; set; }
        /// <summary>
        /// Lấy giá trị từ constant class ElementType
        /// </summary>
        public virtual string ELEMENT_TYPE { get; set; }
        /// <summary>
        /// Lấy giá trị từ constant class BudgetType
        /// </summary>
        public virtual string BUDGET_TYPE { get; set; }
        /// <summary>
        /// Lấy giá trị từ constant class TemplateObjectType
        /// </summary>
        public virtual string OBJECT_TYPE { get; set; }
        /// <summary>
        /// Tên rút gọn khi tạo biểu mẫu
        /// </summary>
        public virtual string ACRONYM_NAME { get; set; }
    }
}
