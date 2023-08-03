using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailOtherCostCFRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_OTHER_COST_CF, T_MD_COST_CF_ELEMENT, T_MD_OTHER_PROFIT_CENTER>, ITemplateDetailOtherCostCFRepo
    {
        public TemplateDetailOtherCostCFRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
