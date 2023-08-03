using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailCostPLRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_COST_PL, T_MD_COST_PL_ELEMENT, T_MD_COST_CENTER>, ITemplateDetailCostPLRepo
    {
        public TemplateDetailCostPLRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
