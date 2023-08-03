using System.Collections.Generic;

namespace SMO.Service.Class.Base
{
    public interface IReviewCenterViewModelBase<T> where T : IElementReviewCenterBase
    {
        IList<T> Elements { get; set; }
        string OrgCode { get; set; }
        string UserControl { get; set; }
        string UserCouncil { get; set; }
        int Version { get; set; }
        int Year { get; set; }
    }
}