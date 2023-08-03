using SMO.Core.Entities;
using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.ServiceInterface.BP.RevenuePL
{
    public interface IRevenuePLService
    {
        IList<T_MD_REVENUE_PL_ELEMENT> GetDataRevenuePreview(
            out IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> detailRevenueElements,
            string templateId,
            string centerCode = "",
            int? year = null,
            int? version = null,
            bool? isHasValue = null);
        IList<T_MD_REVENUE_PL_ELEMENT> PreparePureList(IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> detailRevenueElements, int year);
        IList<T_MD_REVENUE_PL_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> detailRevenueElements,
        string templateId,
        int year,
        string centerCode = "",
        bool ignoreAuth = false);

        IList<T_MD_REVENUE_PL_ELEMENT> PreparePureListForTemplate(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> detailRevenueElements,
            string templateId,
            int year);
        IList<T_MD_REVENUE_PL_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> detailRevenueElements,
            IList<string> centerCodes,
            int year);
        IList<T_MD_REVENUE_PL_ELEMENT> SummarySumUpCenter(
           out IList<T_BP_REVENUE_PL_DATA> plDataRevenueElements,
           int year,
           string centerCode,
           int? version,
           bool? isHasValue = null,
           string templateId = "");
        IList<T_MD_REVENUE_PL_ELEMENT> SummaryCenterOut(out IList<T_BP_REVENUE_PL_DATA> plDataRevenueElements, string centerCode, int year, int? version, bool? isHasValue = null);
        IList<T_MD_REVENUE_PL_ELEMENT> SummaryCenterVersion(out IList<T_BP_REVENUE_PL_DATA> plDataRevenueElements, string centerCode, int year, int? version, bool isDrillDown = false);
    }
}
