using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service
{
    public partial class BaseService
    {
        public string ErrorDetail { get; set; }
        public string ErrorMessage { get; set; }
        public string MesseageCode { get; set; }
        public Exception Exception { get; set; }

        /// <summary>
        /// Trạng thái sau khi thực hiện câu lệnh
        /// Nếu ErrorMessage != 'null' thì State = false
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// Giá trị trả lại
        /// </summary>
        public string ValueReturn { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// Tên column được sort
        /// </summary>
        public string SortColumnName { get; set; }

        /// <summary>
        /// Sort theo thứ tự tăng dần hay giảm dần
        /// </summary>
        public string SortDirection { get; set; }

        /// <summary>
        /// Danh sách ID được chọn
        /// </summary>
        public string ListSelected { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Kiểu cập nhật dữ liệu: insert/update
        /// </summary>
        public string TypeAction { get; set; }
        /// <summary>
        /// Số row trên một trang
        /// </summary>
        public int NumerRecordPerPage { get; set; }

        public string ViewId { get; set; }
        public string FormId { get; set; }

        public BaseService()
        {
            Exception = null;
            State = true;
            Page = 1;
            NumerRecordPerPage = 50;
            TotalRecord = 0;
            SortColumnName = string.Empty;
            SortDirection = string.Empty;
            FormId = "";
            ViewId = "";
        }

        /// <summary>
        /// Merge two list type T and distinct it
        /// Must to implements Equal in entity class
        /// </summary>
        /// <param name="lst1">First list</param>
        /// <param name="lst2">Second list</param>
        /// <returns>List of <T> has merged</returns>
        public virtual IList<T> SummaryElement<T>(IList<T> lst1, IList<T> lst2) where T : class
        {
            return lst1.ToList().Union(lst2).ToList();
        }
    }
}
