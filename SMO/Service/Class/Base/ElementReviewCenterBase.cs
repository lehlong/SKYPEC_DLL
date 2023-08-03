using SMO.Core.Common;

using System.Collections.Generic;

namespace SMO.Service.Class.Base
{
    public class ElementReviewCenterBase : CoreElement, IElementReviewCenterBase
    {
        public ElementReviewCenterBase(CoreElement element,
                                              IList<string> success,
                                              IList<string> failure,
                                              IList<string> notReviewed,
                                              bool? status,
                                              int comments,
                                              int commentsInOrg)
        {
            Success = success;
            Failure = failure;
            NotReviewed = notReviewed;
            CENTER_CODE = CENTER_CODE;
            CODE = element.CODE;
            Values = element.Values;
            IS_GROUP = element.IS_GROUP;
            DESCRIPTION = element.DESCRIPTION;
            NAME = element.NAME;
            LEVEL = element.LEVEL;
            Status = status;
            IsChildren = element.IsChildren;
            Comments = comments;
            CommentsInOrg = commentsInOrg;
        }
        /// <summary>
        /// List username thẩm định đạt khoản mục
        /// </summary>
        public IList<string> Success { get; set; }
        /// <summary>
        /// List username thẩm định không đạt khoản mục
        /// </summary>
        public IList<string> Failure { get; set; }
        /// <summary>
        /// List username chưa thẩm định khoản mục
        /// </summary>
        public IList<string> NotReviewed { get; set; }
        public bool? Status { get; set; }
        public int Comments { get; set; }
        public int CommentsInOrg { get; set; }
    }
}
