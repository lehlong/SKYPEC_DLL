using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailRevenueCFRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_REVENUE_CF, T_MD_REVENUE_CF_ELEMENT, T_MD_PROFIT_CENTER>, ITemplateDetailRevenueCF
    {
        public TemplateDetailRevenueCFRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
