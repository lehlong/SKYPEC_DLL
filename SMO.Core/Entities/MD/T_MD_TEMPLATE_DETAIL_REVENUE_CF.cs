﻿using SMO.Core.Common;

namespace SMO.Core.Entities.MD
{
    public class T_MD_TEMPLATE_DETAIL_REVENUE_CF : BaseTemplateDetail<T_MD_REVENUE_CF_ELEMENT, T_MD_PROFIT_CENTER>
    {
        public T_MD_TEMPLATE_DETAIL_REVENUE_CF() : base()
        {

        }
        public T_MD_TEMPLATE_DETAIL_REVENUE_CF(string pkid, string templateCode, string elementCode, string centerCode, int year) : base(pkid, templateCode, elementCode, centerCode, year)
        {
        }
        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_BP_REVENUE_CF_DATA PlData { get; set; }
    }
}