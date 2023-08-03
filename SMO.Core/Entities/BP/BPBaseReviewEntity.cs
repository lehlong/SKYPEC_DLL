using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.Core.Entities.BP
{
    public class BPBaseReviewEntity<TResult> : BaseEntity
    {
        public BPBaseReviewEntity()
        {
            Results = new List<TResult>();
        }

        public virtual string PKID { get; set; }
        /// <summary>
        /// Thẩm định cho đơn vị nào (tập đoàn)
        /// </summary>
        public virtual string ORG_CODE { get; set; }
        /// <summary>
        /// Năm thẩm định
        /// </summary>
        public virtual int TIME_YEAR { get; set; }
        /// <summary>
        /// Version thẩm định
        /// </summary>
        public virtual int DATA_VERSION { get; set; }
        /// <summary>
        /// Người thẩm định
        /// </summary>
        public virtual string REVIEW_USER { get; set; }
        /// <summary>
        /// Đã kết thúc quá trình tổng kiểm soát chưa
        /// </summary>
        public virtual bool IS_END { get; set; }
        /// <summary>
        /// Có phải là tổng kiểm soát hay không
        /// </summary>
        public virtual bool IS_SUMMARY { get; set; }

        public virtual T_MD_COST_CENTER Organize { get; set; }
        public virtual T_AD_USER UserReview { get; set; }
        public virtual IList<TResult> Results { get; set; }
    }
}
