using SMO.Core.Entities.MD;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập (required)", AllowEmptyStrings = false)]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [MaxLength(length: 20, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        public virtual string USER_NAME { get; set; }

        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{10,}$", ErrorMessage = "Mật khẩu có ít nhất 10 kí tự. Bao gồm cả số,chữ thường và chữ hoa. (The password's must contain at least 10 characters  and must include at least one upper case letter, one lower case letter, and one numeric digit)")]
        public virtual string PASSWORD { get; set; }

        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{10,}$", ErrorMessage = "Mật khẩu có ít nhất 10 kí tự. Bao gồm cả số,chữ thường và chữ hoa.  (The password's must contain at least 10 characters  and must include at least one upper case letter, one lower case letter, and one numeric digit)")]
        public virtual string RETRY_PASSWORD { get; set; }

        //[RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách ()")]
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string OLD_PASSWORD { get; set; }

        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string FULL_NAME { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string PHONE { get; set; }
        public virtual string NOTES { get; set; }
        public virtual string LANGUAGE { get; set; }
        public virtual string USER_TYPE { get; set; }
        public virtual string TITLE_ID { get; set; }
        public virtual string ORGANIZE_CODE { get; set; }
        public virtual string ACCOUNT_AD { get; set; }
        public virtual bool IS_MODIFY_RIGHT { get; set; }
        public virtual bool IS_IGNORE_USER { get; set; }
        public virtual bool IS_CHANGE_LANG { get; set; }
        public virtual bool IS_LOGIN_AD { get; set; }
        /// <summary>
        /// Chức danh
        /// </summary>
        public virtual string TITLE { get; set; }   // tên chức danh

        public virtual DateTime? LAST_CHANGE_PASS_DATE { get; set; }
        public virtual ISet<T_AD_USER_USER_GROUP> ListUserUserGroup { get; set; }
        public virtual ISet<T_AD_USER_RIGHT> ListUserRight { get; set; }
        public virtual ISet<T_AD_USER_ROLE> ListUserRole { get; set; }
        public virtual ISet<T_AD_USER_HISTORY> ListUserHistory { get; set; }
        public virtual ISet<T_AD_USER_ORG> ListUserOrg { get; set; }
        private T_MD_COST_CENTER _Organize;
        public virtual T_MD_COST_CENTER Organize
        {
            get
            {
                if (_Organize == null)
                {
                    _Organize = new T_MD_COST_CENTER();
                }
                return _Organize;
            }
            set
            {
                _Organize = value;
            }
        }
        public T_AD_USER()
        {
            ListUserUserGroup = new HashSet<T_AD_USER_USER_GROUP>(new List<T_AD_USER_USER_GROUP>());
            ListUserRight = new HashSet<T_AD_USER_RIGHT>(new List<T_AD_USER_RIGHT>());
            ListUserRole = new HashSet<T_AD_USER_ROLE>(new List<T_AD_USER_ROLE>());
            ListUserHistory = new HashSet<T_AD_USER_HISTORY>(new List<T_AD_USER_HISTORY>());
            ListUserOrg = new HashSet<T_AD_USER_ORG>(new List<T_AD_USER_ORG>());
            Organize = new T_MD_COST_CENTER();
        }
    }
}
