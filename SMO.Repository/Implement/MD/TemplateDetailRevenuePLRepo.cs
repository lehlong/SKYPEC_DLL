using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class TemplateDetailRevenuePLRepo : GenericTemplateDetailRepository<T_MD_TEMPLATE_DETAIL_REVENUE_PL, T_MD_REVENUE_PL_ELEMENT, T_MD_PROFIT_CENTER>, ITemplateDetailRevenuePL
    {
        public TemplateDetailRevenuePLRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
