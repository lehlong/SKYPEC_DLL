using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailCostCFRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_COST_CF, T_MD_COST_CF_ELEMENT, T_MD_COST_CENTER>, ITemplateDetailCostCFRepo
    {
        public TemplateDetailCostCFRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
