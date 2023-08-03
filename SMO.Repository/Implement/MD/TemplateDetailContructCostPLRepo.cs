using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailContructCostPLRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL, T_MD_COST_PL_ELEMENT, T_MD_INTERNAL_ORDER>, ITemplateDetailContructCostPLRepo
    {
        public TemplateDetailContructCostPLRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
