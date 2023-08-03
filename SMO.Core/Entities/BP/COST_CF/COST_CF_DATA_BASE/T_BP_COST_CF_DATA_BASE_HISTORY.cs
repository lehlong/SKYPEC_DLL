using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.Core.Entities.BP.COST_CF.COST_CF_DATA_BASE
{
    public class T_BP_COST_CF_DATA_BASE_HISTORY : BaseEntity
    {
        public virtual string PKID { get; set; }

        public virtual string ORG_CODE { get; set; }
        public virtual string COST_CENTER_CODE { get; set; }
        public virtual string TEMPLATE_CODE { get; set; }
        public virtual string COST_CF_ELEMENT_CODE { get; set; }
        public virtual int VERSION { get; set; }
        public virtual int TIME_YEAR { get; set; }
        public virtual string MATERIAL { get; set; }
        public virtual string UNIT { get; set; }

        public virtual decimal QUANTITY_M1 { get; set; }
        public virtual string TIME_M1 { get; set; }
        public virtual decimal PRICE_M1 { get; set; }
        public virtual decimal AMOUNT_M1 { get; set; }

        public virtual decimal QUANTITY_M2 { get; set; }
        public virtual string TIME_M2 { get; set; }
        public virtual decimal PRICE_M2 { get; set; }
        public virtual decimal AMOUNT_M2 { get; set; }

        public virtual decimal QUANTITY_M3 { get; set; }
        public virtual string TIME_M3 { get; set; }
        public virtual decimal PRICE_M3 { get; set; }
        public virtual decimal AMOUNT_M3 { get; set; }

        public virtual decimal QUANTITY_M4 { get; set; }
        public virtual string TIME_M4 { get; set; }
        public virtual decimal PRICE_M4 { get; set; }
        public virtual decimal AMOUNT_M4 { get; set; }

        public virtual decimal QUANTITY_M5 { get; set; }
        public virtual string TIME_M5 { get; set; }
        public virtual decimal PRICE_M5 { get; set; }
        public virtual decimal AMOUNT_M5 { get; set; }

        public virtual decimal QUANTITY_M6 { get; set; }
        public virtual string TIME_M6 { get; set; }
        public virtual decimal PRICE_M6 { get; set; }
        public virtual decimal AMOUNT_M6 { get; set; }

        public virtual decimal QUANTITY_M7 { get; set; }
        public virtual string TIME_M7 { get; set; }
        public virtual decimal PRICE_M7 { get; set; }
        public virtual decimal AMOUNT_M7 { get; set; }

        public virtual decimal QUANTITY_M8 { get; set; }
        public virtual string TIME_M8 { get; set; }
        public virtual decimal PRICE_M8 { get; set; }
        public virtual decimal AMOUNT_M8 { get; set; }

        public virtual decimal QUANTITY_M9 { get; set; }
        public virtual string TIME_M9 { get; set; }
        public virtual decimal PRICE_M9 { get; set; }
        public virtual decimal AMOUNT_M9 { get; set; }

        public virtual decimal QUANTITY_M10 { get; set; }
        public virtual string TIME_M10 { get; set; }
        public virtual decimal PRICE_M10 { get; set; }
        public virtual decimal AMOUNT_M10 { get; set; }

        public virtual decimal QUANTITY_M11 { get; set; }
        public virtual string TIME_M11 { get; set; }
        public virtual decimal PRICE_M11 { get; set; }
        public virtual decimal AMOUNT_M11 { get; set; }

        public virtual decimal QUANTITY_M12 { get; set; }
        public virtual string TIME_M12 { get; set; }
        public virtual decimal PRICE_M12 { get; set; }
        public virtual decimal AMOUNT_M12 { get; set; }

        #region Ngân sách dự phòng
        public virtual decimal AMOUNT_YEAR_PREVENTIVE { get; set; }
        public virtual decimal AMOUNT_YEAR { get; set; }

        #endregion

        public virtual string DESCRIPTION { get; set; }

        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_MD_COST_CF_ELEMENT CostElement { get; set; }
        public virtual T_MD_COST_CENTER CostCenter { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }

        public override bool Equals(object obj)
        {
            return obj is T_BP_COST_CF_DATA_BASE_HISTORY history &&
                   PKID == history.PKID &&
                   ORG_CODE == history.ORG_CODE &&
                   COST_CENTER_CODE == history.COST_CENTER_CODE &&
                   TEMPLATE_CODE == history.TEMPLATE_CODE &&
                   COST_CF_ELEMENT_CODE == history.COST_CF_ELEMENT_CODE &&
                   VERSION == history.VERSION &&
                   TIME_YEAR == history.TIME_YEAR &&
                   MATERIAL == history.MATERIAL &&
                   UNIT == history.UNIT;
        }

        public override int GetHashCode()
        {
            int hashCode = -245565264;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PKID);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ORG_CODE);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(COST_CENTER_CODE);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TEMPLATE_CODE);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(COST_CF_ELEMENT_CODE);
            hashCode = hashCode * -1521134295 + VERSION.GetHashCode();
            hashCode = hashCode * -1521134295 + TIME_YEAR.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MATERIAL);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UNIT);
            return hashCode;
        }

        public static explicit operator T_BP_COST_CF_DATA_BASE_HISTORY(T_BP_COST_CF_DATA_BASE v)
        {
            return new T_BP_COST_CF_DATA_BASE_HISTORY
            {
                PKID = v.PKID,
                ORG_CODE = v.ORG_CODE,
                COST_CENTER_CODE = v.COST_CENTER_CODE,
                TEMPLATE_CODE = v.TEMPLATE_CODE,
                COST_CF_ELEMENT_CODE = v.COST_CF_ELEMENT_CODE,
                VERSION = v.VERSION,
                TIME_YEAR = v.TIME_YEAR,
                MATERIAL = v.MATERIAL,
                UNIT = v.UNIT,
                
                QUANTITY_M1 = v.QUANTITY_M1,
                PRICE_M1 = v.PRICE_M1,
                AMOUNT_M1 = v.AMOUNT_M1,
                TIME_M1 = v.TIME_M1,

                QUANTITY_M2 = v.QUANTITY_M2,
                PRICE_M2 = v.PRICE_M2,
                AMOUNT_M2 = v.AMOUNT_M2,
                TIME_M2 = v.TIME_M2,

                QUANTITY_M3 = v.QUANTITY_M3,
                PRICE_M3 = v.PRICE_M3,
                AMOUNT_M3 = v.AMOUNT_M3,
                TIME_M3 = v.TIME_M3,

                QUANTITY_M4 = v.QUANTITY_M4,
                PRICE_M4 = v.PRICE_M4,
                AMOUNT_M4 = v.AMOUNT_M4,
                TIME_M4 = v.TIME_M4,

                QUANTITY_M5 = v.QUANTITY_M5,
                PRICE_M5 = v.PRICE_M5,
                AMOUNT_M5 = v.AMOUNT_M5,
                TIME_M5 = v.TIME_M5,

                QUANTITY_M6 = v.QUANTITY_M6,
                PRICE_M6 = v.PRICE_M6,
                AMOUNT_M6 = v.AMOUNT_M6,
                TIME_M6 = v.TIME_M6,

                QUANTITY_M7 = v.QUANTITY_M7,
                PRICE_M7 = v.PRICE_M7,
                AMOUNT_M7 = v.AMOUNT_M7,
                TIME_M7 = v.TIME_M7,

                QUANTITY_M8 = v.QUANTITY_M8,
                PRICE_M8 = v.PRICE_M8,
                AMOUNT_M8 = v.AMOUNT_M8,
                TIME_M8 = v.TIME_M8,

                QUANTITY_M9 = v.QUANTITY_M9,
                PRICE_M9 = v.PRICE_M9,
                AMOUNT_M9 = v.AMOUNT_M9,
                TIME_M9 = v.TIME_M9,

                QUANTITY_M10 = v.QUANTITY_M10,
                PRICE_M10 = v.PRICE_M10,
                AMOUNT_M10 = v.AMOUNT_M10,
                TIME_M10 = v.TIME_M10,

                QUANTITY_M11 = v.QUANTITY_M11,
                PRICE_M11 = v.PRICE_M11,
                AMOUNT_M11 = v.AMOUNT_M11,
                TIME_M11 = v.TIME_M11,

                QUANTITY_M12 = v.QUANTITY_M12,
                PRICE_M12 = v.PRICE_M12,
                AMOUNT_M12 = v.AMOUNT_M12,
                TIME_M12 = v.TIME_M12,

                AMOUNT_YEAR = v.AMOUNT_YEAR,
                AMOUNT_YEAR_PREVENTIVE = v.AMOUNT_YEAR_PREVENTIVE,

                DESCRIPTION = v.DESCRIPTION,
            };
        }


    }
}
