namespace SMO.Service.Class
{
    public class ReviewDataCenterModel
    {
        /// <summary>
        /// Người thẩm định. Để trống nếu muốn hiển thị tất cả
        /// </summary>
        public string ORG_CODE { get; set; }
        /// <summary>
        /// Năm thẩm định
        /// </summary>
        public int YEAR { get; set; }
        /// <summary>
        /// Version thẩm định
        /// </summary>
        public int? VERSION { get; set; }
        /// <summary>
        /// Dữ liệu đã đạt
        /// </summary>
        public bool IS_COMPLETED { get; set; }
        /// <summary>
        /// Dữ liệu chưa đạt
        /// </summary>
        public bool IS_NOT_COMPLETED { get; set; }
        /// <summary>
        /// Hiển thị dữ liệu của hội đồng ngân sách
        /// </summary>
        public bool IS_COUNCIL_BUDGET { get; set; }
        /// <summary>
        /// Hiển thị dữ liệu của tổng kiểm soát
        /// </summary>
        public bool IS_CONTROL { get; set; }
        /// <summary>
        /// Tỷ giá chuyển đổi so với đồng VN
        /// </summary>
        public decimal? EXCHANGE_RATE { get; set; } = 1;
        /// <summary>
        /// Mã đơn vị tiền
        /// </summary>
        public string EXCHANGE_TYPE { get; set; } = "VND";  // fix value is VND
    }
}
