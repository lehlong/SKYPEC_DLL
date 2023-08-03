using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailContructCostCFRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF, T_MD_COST_CF_ELEMENT, T_MD_INTERNAL_ORDER>, ITemplateDetailContructCostCFRepo
    {
        public TemplateDetailContructCostCFRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
