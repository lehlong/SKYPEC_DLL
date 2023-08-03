using System.Collections.Generic;

namespace SMO.Service.Class.Base
{
    public class ReviewCenterViewModelBase<T> : IReviewCenterViewModelBase<T> where T : IElementReviewCenterBase
    {
        public ReviewCenterViewModelBase()
        {
            Elements = new List<T>();
        }
        public IList<T> Elements { get; set; }
        public string OrgCode { get; set; }
        public int Year { get; set; }
        public int Version { get; set; }
        public string UserControl { get; set; }
        public string UserCouncil { get; set; }
    }
}
