using SMO.Core.Common;

namespace SMO.Core.Entities.MD
{
    public class T_MD_TEMPLATE_DETAIL_OTHER_COST_PL : BaseTemplateDetail<T_MD_COST_PL_ELEMENT, T_MD_OTHER_PROFIT_CENTER>
    {
        public T_MD_TEMPLATE_DETAIL_OTHER_COST_PL()
        {
        }

        public T_MD_TEMPLATE_DETAIL_OTHER_COST_PL(string pkid, string templateCode, string elementCode, string centerCode, int year) : base(pkid, templateCode, elementCode, centerCode, year)
        {
        }
        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_BP_OTHER_COST_PL_DATA PLData { get; set; }

    }
}
