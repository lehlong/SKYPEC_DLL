using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailOtherCostPLRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_OTHER_COST_PL, T_MD_COST_PL_ELEMENT, T_MD_OTHER_PROFIT_CENTER>, ITemplateDetailOtherCostPLRepo
    {
        public TemplateDetailOtherCostPLRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
