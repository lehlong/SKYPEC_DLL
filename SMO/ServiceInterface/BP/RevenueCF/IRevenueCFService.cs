using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Service.Class;

using System.Collections.Generic;

namespace SMO.ServiceInterface.BP.RevenueCF
{
    public interface IRevenueCFService
    {
        IList<T_MD_REVENUE_CF_ELEMENT> GetDataRevenuePreview(
            out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            string templateId,
            string centerCode = "",
            int? year = null,
            int? version = null,
            bool? isHasValue = null);
        IList<T_MD_REVENUE_CF_ELEMENT> PreparePureList(IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements, int year);
        IList<T_MD_REVENUE_CF_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
        string templateId,
        int year,
        string centerCode = "",
        bool ignoreAuth = false);

        IList<T_MD_REVENUE_CF_ELEMENT> PreparePureListForTemplate(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            string templateId,
            int year);
        IList<T_MD_REVENUE_CF_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            IList<string> centerCodes,
            int year);
        IList<T_MD_REVENUE_CF_ELEMENT> SummarySumUpCenter(
           out IList<T_BP_REVENUE_CF_DATA> plDataRevenueElements,
           int year,
           string centerCode,
           int? version,
           bool? isHasValue = null,
           string templateId = "");
        IList<T_MD_REVENUE_CF_ELEMENT> SummaryCenterOut(out IList<T_BP_REVENUE_CF_DATA> plDataRevenueElements, string centerCode, int year, int? version, bool? isHasValue = null);
        IList<T_MD_REVENUE_CF_ELEMENT> SummaryCenterVersion(out IList<T_BP_REVENUE_CF_DATA> plDataRevenueElements, string centerCode, int year, int? version, bool isDrillDown = false);
        IList<T_MD_REVENUE_CF_ELEMENT> GetDataRevenue(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements, out IList<T_BP_REVENUE_CF_DATA> detailRevenueData, out bool isDrillDownApply, ViewDataCenterModel model);
    }
}
