using SMO.Service.Class.Base;

namespace SMO.Models
{
    public class SummaryReviewDataCenterModel
    {
        public SummaryReviewDataCenterModel()
        {

        }
        public SummaryReviewDataCenterModel(IReviewCenterViewModelBase<IElementReviewCenterBase> reviewCenterViewModel)
        {
            ReviewCenterViewModel = reviewCenterViewModel;
        }

        public IReviewCenterViewModelBase<IElementReviewCenterBase> ReviewCenterViewModel { get; set; }
    }
}
