using SMO.Core.Entities;
using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.ServiceInterface.BP.ContructCostPL
{
    public interface IContructCostPLService
    {
        IList<T_MD_COST_PL_ELEMENT> GetDataCostPreview(
            out IList<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL> detailCostElements,
            string templateId,
            string centerCode = "",
            int? year = null,
            int? version = null,
            bool? isHasValue = null);
        IList<T_MD_COST_PL_ELEMENT> PreparePureList(IList<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL> detailCostElements, int year);
        IList<T_MD_COST_PL_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL> detailCostElements,
        string templateId,
        int year,
        string centerCode = "",
        bool ignoreAuth = false);

        IList<T_MD_COST_PL_ELEMENT> PreparePureListForTemplate(out IList<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL> detailCostElements,
            string templateId,
            int year);
        IList<T_MD_COST_PL_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL> detailCostElements,
            IList<string> centerCodes,
            int year);
        IList<T_MD_COST_PL_ELEMENT> SummarySumUpCenter(
           out IList<T_BP_CONTRUCT_COST_PL_DATA> plDataCostElements,
           int year,
           string centerCode,
           int? version,
           bool? isHasValue = null,
           string templateId = "");
        IList<T_MD_COST_PL_ELEMENT> SummaryCenterOut(out IList<T_BP_CONTRUCT_COST_PL_DATA> plDataCostElements, string centerCode, int year, int? version, bool? isHasValue = null);
        IList<T_MD_COST_PL_ELEMENT> SummaryCenterVersion(out IList<T_BP_CONTRUCT_COST_PL_DATA> plDataCostElements, string centerCode, int year, int? version, bool isDrillDown = false);
    }
}
