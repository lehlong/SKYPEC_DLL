using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_MATERIAL : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [MaxLength(length: 50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string TEXT { get; set; }
        public virtual string TYPE { get; set; }
        public virtual string UNIT { get; set; }
        public virtual T_MD_MATERIAL_TYPE Types { get; set; }
        public virtual T_MD_UNIT Units { get; set; }
    }
}
