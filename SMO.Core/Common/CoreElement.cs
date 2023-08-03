using SMO.Core.Entities.MD;

using System.Collections.Generic;
using System.Globalization;

namespace SMO.Core.Common
{
    public abstract class CoreElement : CoreTree, ICoreElement
    {
        public CoreElement() : base()
        {

        }
        public virtual string PKID { get; set; }
        public virtual bool IS_GROUP { get; set; }
        public virtual bool HasAssignValue { get; set; }
        public virtual bool IsChildren { get; set; }
        public virtual bool IsSumUp { get; set; }
        public virtual bool IsBase { get; set; }
        public virtual string TEMPLATE_CODE_PURE { get; protected set; }

        public virtual string[] ValuesBaseString { get; set; }

        /// <summary>
        /// Ngân sách gốc
        /// </summary>
        public virtual decimal[] Values { get; set; }

        public virtual string TEMPLATE_CODE { get; set; }
        public virtual int VERSION { get; set; }
        /// <summary>
        /// Số lượng comments của khoản mục | số lượng comments của khoản mục con
        /// </summary>
        public virtual string NUMBER_COMMENTS { get; set; }
        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual string ORG_CODE { get; set; }
        public virtual string ORG_NAME { get; set; }
        public virtual string DESCRIPTION { get; set; }

        public virtual int LEVEL { get; set; }      // property to determine level of element. Higher is child
        public virtual int TIME_YEAR { get; set; }
        public virtual string CENTER_CODE { get; set; }     // property to determine in the same center

        public override bool Equals(object obj)
        {
            return obj is CoreElement element &&
                   TIME_YEAR == element.TIME_YEAR &&
                   CODE == element.CODE;
        }

        public override int GetHashCode()
        {
            var hashCode = 2051980312;
            hashCode = hashCode * -1521134295 + TIME_YEAR.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CODE);
            return hashCode;
        }

        internal static string HandleValueBaseString(decimal? quantity, decimal? price, decimal amount, string time = "")
        {
            var formatNumber = "#,#0.##";
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            return
                (quantity.HasValue ? $"SL: { quantity.Value.ToString(formatNumber, culture)} " : string.Empty) +
                (!string.IsNullOrEmpty(time) ? $"\nTG: {time}" : string.Empty) +
                (price.HasValue ? $"\nĐG: {price.Value.ToString(formatNumber, culture)}" : string.Empty) +
                $"\nTT: {amount.ToString(formatNumber, culture)}";
        }

    }
}
