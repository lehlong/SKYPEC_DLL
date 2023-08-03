using SMO.Core.Entities;

using System.Collections.Generic;

namespace SMO.Core.Common
{
    public class BaseTemplateDetail<TElement, TCenter> : BaseEntity where TCenter : CoreCenter where TElement : CoreElement
    {
        public BaseTemplateDetail() : base()
        {

        }
        public BaseTemplateDetail(string pkid, string templateCode, string elementCode, string centerCode, int year) : base()
        {
            PKID = pkid;
            TEMPLATE_CODE = templateCode;
            ELEMENT_CODE = elementCode;
            CENTER_CODE = centerCode;
            TIME_YEAR = year;
        }
        public virtual string PKID { get; set; }
        public virtual string TEMPLATE_CODE { get; set; }
        public virtual string ELEMENT_CODE { get; set; }
        public virtual string CENTER_CODE { get; set; }
        public virtual TElement Element { get; set; }
        public virtual TCenter Center { get; set; }
        public virtual int TIME_YEAR { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BaseTemplateDetail<TElement, TCenter> detail &&
                   TEMPLATE_CODE == detail.TEMPLATE_CODE &&
                   ELEMENT_CODE == detail.ELEMENT_CODE &&
                   TIME_YEAR == detail.TIME_YEAR &&
                   CENTER_CODE == detail.CENTER_CODE;
        }

        public override int GetHashCode()
        {
            var hashCode = -1696976938;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TEMPLATE_CODE);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ELEMENT_CODE);
            hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(TIME_YEAR);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CENTER_CODE);
            return hashCode;
        }
    }
}
