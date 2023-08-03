using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_CURRENCY : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [MaxLength(length: 50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string TEXT { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập")]
        [Range(1, int.MaxValue, ErrorMessage = "Tỉ giá không phù hợp")]
        public virtual decimal EXCHANGE_RATE { get; set; }
    }
}
