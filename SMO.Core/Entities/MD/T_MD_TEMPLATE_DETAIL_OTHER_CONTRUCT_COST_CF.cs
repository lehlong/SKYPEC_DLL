using SMO.Core.Common;

namespace SMO.Core.Entities.MD
{
    public class T_MD_TEMPLATE_DETAIL_OTHER_COST_CF : BaseTemplateDetail<T_MD_COST_CF_ELEMENT, T_MD_OTHER_PROFIT_CENTER>
    {
        public T_MD_TEMPLATE_DETAIL_OTHER_COST_CF()
        {
        }

        public T_MD_TEMPLATE_DETAIL_OTHER_COST_CF(string pkid, string templateCode, string elementCode, string centerCode, int year) : base(pkid, templateCode, elementCode, centerCode, year)
        {
        }
        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_BP_OTHER_COST_CF_DATA PLData { get; set; }

    }
}
