using SMO.Core.Common;
using SMO.Core.Entities.BP.CONTRUCT_COST_PL.CONTRUCT_COST_PL_DATA_BASE;
using SMO.Core.Entities.BP.COST_PL.COST_PL_DATA_BASE;
using SMO.Core.Entities.BP.OTHER_COST_PL.OTHER_COST_PL_DATA_BASE;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SMO.Core.Entities.MD
{
    [DebuggerDisplay("{DESCRIPTION}", Name = "DESCRIPTION: ")]
    public class T_MD_COST_PL_ELEMENT : CoreElement, ICloneable
    {
        public T_MD_COST_PL_ELEMENT() : base()
        {
            Values = new decimal[14];
            //ValuesBaseString = new string[14];
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            return obj is T_MD_COST_PL_ELEMENT element &&
                   TIME_YEAR == element.TIME_YEAR &&
                   Values == element.Values &&
                   CODE == element.CODE;
        }

        public override int GetHashCode()
        {
            var hashCode = 2051980312;
            hashCode = hashCode * -1521134295 + TIME_YEAR.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<decimal[]>.Default.GetHashCode(Values);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CODE);
            return hashCode;
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_COST_PL_DATA_HISTORY v)
        {
            if (v == null)
            {
                return new T_MD_COST_PL_ELEMENT();
            }
            return new T_MD_COST_PL_ELEMENT
            {
                TEMPLATE_CODE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                ORG_NAME = string.IsNullOrEmpty(v.TEMPLATE_CODE) ? v.CostCenter.NAME : $"Đơn vị nộp: {v.Template.Organize.NAME}\nMẫu: {v.Template.NAME}({v.TEMPLATE_CODE})\nTrạng thái: {Approve_Status.GetStatusText(v.STATUS)}",
                CENTER_CODE = v.COST_CENTER_CODE,
                Values = new decimal[14]
                {
                    v.VALUE_JAN ?? 0,
                    v.VALUE_FEB ?? 0,
                    v.VALUE_MAR ?? 0,
                    v.VALUE_APR ?? 0,
                    v.VALUE_MAY ?? 0,
                    v.VALUE_JUN ?? 0,
                    v.VALUE_JUL ?? 0,
                    v.VALUE_AUG ?? 0,
                    v.VALUE_SEP ?? 0,
                    v.VALUE_OCT ?? 0,
                    v.VALUE_NOV ?? 0,
                    v.VALUE_DEC ?? 0,
                    v.VALUE_SUM_YEAR ?? 0,
                    v.VALUE_SUM_YEAR_PREVENTIVE ?? 0
                },
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_COST_PL_DATA v)
        {
            if (v == null)
            {
                return new T_MD_COST_PL_ELEMENT();
            }
            return new T_MD_COST_PL_ELEMENT
            {
                TEMPLATE_CODE = v.TEMPLATE_CODE,
                ORG_NAME = string.IsNullOrEmpty(v.TEMPLATE_CODE) ? v.CostCenter.NAME : $"Đơn vị nộp: {v.Template.Organize.NAME}\nMẫu: {v.Template.NAME}({v.TEMPLATE_CODE})\nTrạng thái: {Approve_Status.GetStatusText(v.STATUS)}",
                ORG_CODE = v.ORG_CODE,
                CENTER_CODE = v.COST_CENTER_CODE,
                Values = new decimal[14]
                {
                    v.VALUE_JAN ?? 0,
                    v.VALUE_FEB ?? 0,
                    v.VALUE_MAR ?? 0,
                    v.VALUE_APR ?? 0,
                    v.VALUE_MAY ?? 0,
                    v.VALUE_JUN ?? 0,
                    v.VALUE_JUL ?? 0,
                    v.VALUE_AUG ?? 0,
                    v.VALUE_SEP ?? 0,
                    v.VALUE_OCT ?? 0,
                    v.VALUE_NOV ?? 0,
                    v.VALUE_DEC ?? 0,
                    v.VALUE_SUM_YEAR ?? 0,
                    v.VALUE_SUM_YEAR_PREVENTIVE ?? 0
                },
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_CONTRUCT_COST_PL_DATA_HISTORY v)
        {
            if (v == null)
            {
                return new T_MD_COST_PL_ELEMENT();
            }
            return new T_MD_COST_PL_ELEMENT
            {
                TEMPLATE_CODE = v.TEMPLATE_CODE + v.INTERNAL_ORDER_CODE,
                TEMPLATE_CODE_PURE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                ORG_NAME = $"{v.INTERNAL_ORDER_CODE} - {v.InternalOrder.NAME}\nMẫu nộp: {v.TEMPLATE_CODE}\nĐơn vị nộp: {v.Template.Organize.NAME}\nTrạng thái: {Approve_Status.GetStatusText(v.STATUS)}",
                CENTER_CODE = v.ORG_CODE,
                Values = new decimal[14]
                {
                    v.VALUE_JAN ?? 0,
                    v.VALUE_FEB ?? 0,
                    v.VALUE_MAR ?? 0,
                    v.VALUE_APR ?? 0,
                    v.VALUE_MAY ?? 0,
                    v.VALUE_JUN ?? 0,
                    v.VALUE_JUL ?? 0,
                    v.VALUE_AUG ?? 0,
                    v.VALUE_SEP ?? 0,
                    v.VALUE_OCT ?? 0,
                    v.VALUE_NOV ?? 0,
                    v.VALUE_DEC ?? 0,
                    v.VALUE_SUM_YEAR ?? 0,
                    v.VALUE_SUM_YEAR_PREVENTIVE ?? 0
                },
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_CONTRUCT_COST_PL_DATA v)
        {
            if (v == null)
            {
                return new T_MD_COST_PL_ELEMENT();
            }
            return new T_MD_COST_PL_ELEMENT
            {
                TEMPLATE_CODE = v.TEMPLATE_CODE + v.INTERNAL_ORDER_CODE,
                TEMPLATE_CODE_PURE = v.TEMPLATE_CODE,
                ORG_NAME = $"{v.INTERNAL_ORDER_CODE} - {v.InternalOrder.NAME}\nMẫu nộp: {v.TEMPLATE_CODE}\nĐơn vị nộp: {v.Template.Organize.NAME}\nTrạng thái: {Approve_Status.GetStatusText(v.STATUS)}",
                ORG_CODE = v.ORG_CODE,
                CENTER_CODE = v.ORG_CODE,
                Values = new decimal[14]
                {
                    v.VALUE_JAN ?? 0,
                    v.VALUE_FEB ?? 0,
                    v.VALUE_MAR ?? 0,
                    v.VALUE_APR ?? 0,
                    v.VALUE_MAY ?? 0,
                    v.VALUE_JUN ?? 0,
                    v.VALUE_JUL ?? 0,
                    v.VALUE_AUG ?? 0,
                    v.VALUE_SEP ?? 0,
                    v.VALUE_OCT ?? 0,
                    v.VALUE_NOV ?? 0,
                    v.VALUE_DEC ?? 0,
                    v.VALUE_SUM_YEAR ?? 0,
                    v.VALUE_SUM_YEAR_PREVENTIVE ?? 0
                },
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_OTHER_COST_PL_DATA_HISTORY v)
        {
            if (v == null)
            {
                return new T_MD_COST_PL_ELEMENT();
            }
            return new T_MD_COST_PL_ELEMENT
            {
                ORG_CODE = v.ORG_CODE,
                ORG_NAME = string.IsNullOrEmpty(v.TEMPLATE_CODE) ? v.Organize.NAME : $"Công ty: {v.OtherProfitCenter.Company.NAME}\nDự án: {v.OtherProfitCenter.Project.NAME}\nMẫu: {v.TEMPLATE_CODE}\nĐơn vị nộp: {v.Template.Organize.NAME}\nTrạng thái: {Approve_Status.GetStatusText(v.STATUS)}",
                CENTER_CODE = v.ORG_CODE,
                Values = new decimal[14]
                {
                    v.VALUE_JAN ?? 0,
                    v.VALUE_FEB ?? 0,
                    v.VALUE_MAR ?? 0,
                    v.VALUE_APR ?? 0,
                    v.VALUE_MAY ?? 0,
                    v.VALUE_JUN ?? 0,
                    v.VALUE_JUL ?? 0,
                    v.VALUE_AUG ?? 0,
                    v.VALUE_SEP ?? 0,
                    v.VALUE_OCT ?? 0,
                    v.VALUE_NOV ?? 0,
                    v.VALUE_DEC ?? 0,
                    v.VALUE_SUM_YEAR ?? 0,
                    v.VALUE_SUM_YEAR_PREVENTIVE ?? 0
                },
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_OTHER_COST_PL_DATA v)
        {
            if (v == null)
            {
                return new T_MD_COST_PL_ELEMENT();
            }
            return new T_MD_COST_PL_ELEMENT
            {
                TEMPLATE_CODE = v.TEMPLATE_CODE + v.OTHER_PROFIT_CENTER_CODE,
                TEMPLATE_CODE_PURE = v.TEMPLATE_CODE,
                ORG_NAME = string.IsNullOrEmpty(v.TEMPLATE_CODE) ? v.Organize.NAME : $"Công ty: {v.OtherProfitCenter.Company.NAME}\nDự án: {v.OtherProfitCenter.Project.NAME}\nMẫu: {v.TEMPLATE_CODE}\nĐơn vị nộp: {v.Template.Organize.NAME}\nTrạng thái: {Approve_Status.GetStatusText(v.STATUS)}",
                ORG_CODE = v.ORG_CODE,
                CENTER_CODE = v.ORG_CODE,
                Values = new decimal[14]
                {
                    v.VALUE_JAN ?? 0,
                    v.VALUE_FEB ?? 0,
                    v.VALUE_MAR ?? 0,
                    v.VALUE_APR ?? 0,
                    v.VALUE_MAY ?? 0,
                    v.VALUE_JUN ?? 0,
                    v.VALUE_JUL ?? 0,
                    v.VALUE_AUG ?? 0,
                    v.VALUE_SEP ?? 0,
                    v.VALUE_OCT ?? 0,
                    v.VALUE_NOV ?? 0,
                    v.VALUE_DEC ?? 0,
                    v.VALUE_SUM_YEAR ?? 0,
                    v.VALUE_SUM_YEAR_PREVENTIVE ?? 0
                },
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_COST_PL_DATA_BASE v)
        {
            return new T_MD_COST_PL_ELEMENT
            {
                ORG_NAME = $"{v.MATERIAL} ({v.UNIT})",
                TEMPLATE_CODE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                IS_GROUP = false,
                CENTER_CODE = v.COST_CENTER_CODE,
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template,
                IsBase = true,
                ValuesBaseString = new string[15]
                {
                    HandleValueBaseString(quantity: v.QUANTITY_M1, price: v.PRICE_M1, amount: v.AMOUNT_M1),
                    HandleValueBaseString(quantity: v.QUANTITY_M2, price: v.PRICE_M2, amount: v.AMOUNT_M2),
                    HandleValueBaseString(quantity: v.QUANTITY_M3, price: v.PRICE_M3, amount: v.AMOUNT_M3),
                    HandleValueBaseString(quantity: v.QUANTITY_M4, price: v.PRICE_M4, amount: v.AMOUNT_M4),
                    HandleValueBaseString(quantity: v.QUANTITY_M5, price: v.PRICE_M5, amount: v.AMOUNT_M5),
                    HandleValueBaseString(quantity: v.QUANTITY_M6, price: v.PRICE_M6, amount: v.AMOUNT_M6),
                    HandleValueBaseString(quantity: v.QUANTITY_M7, price: v.PRICE_M7, amount: v.AMOUNT_M7),
                    HandleValueBaseString(quantity: v.QUANTITY_M8, price: v.PRICE_M8, amount: v.AMOUNT_M8),
                    HandleValueBaseString(quantity: v.QUANTITY_M9, price: v.PRICE_M9, amount: v.AMOUNT_M9),
                    HandleValueBaseString(quantity: v.QUANTITY_M10, price: v.PRICE_M10, amount: v.AMOUNT_M10),
                    HandleValueBaseString(quantity: v.QUANTITY_M11, price: v.PRICE_M11, amount: v.AMOUNT_M11),
                    HandleValueBaseString(quantity: v.QUANTITY_M12, price: v.PRICE_M12, amount: v.AMOUNT_M12),
                    HandleValueBaseString(quantity: v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1, price: null, amount: v.AMOUNT_YEAR),

                    HandleValueBaseString(quantity: null, price: null, amount: v.AMOUNT_YEAR_PREVENTIVE),

                    HandleValueBaseString(quantity: (v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1)/12, price: null, amount: v.AMOUNT_YEAR/12),
                },
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_COST_PL_DATA_BASE_HISTORY v)
        {
            return new T_MD_COST_PL_ELEMENT
            {
                ORG_NAME = $"{v.MATERIAL} ({v.UNIT})",
                TEMPLATE_CODE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                CENTER_CODE = v.COST_CENTER_CODE,
                VERSION = v.VERSION,
                IS_GROUP = false,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template,
                IsBase = true,
                ValuesBaseString = new string[15]
                {
                    HandleValueBaseString(quantity: v.QUANTITY_M1, price: v.PRICE_M1, amount: v.AMOUNT_M1),
                    HandleValueBaseString(quantity: v.QUANTITY_M2, price: v.PRICE_M2, amount: v.AMOUNT_M2),
                    HandleValueBaseString(quantity: v.QUANTITY_M3, price: v.PRICE_M3, amount: v.AMOUNT_M3),
                    HandleValueBaseString(quantity: v.QUANTITY_M4, price: v.PRICE_M4, amount: v.AMOUNT_M4),
                    HandleValueBaseString(quantity: v.QUANTITY_M5, price: v.PRICE_M5, amount: v.AMOUNT_M5),
                    HandleValueBaseString(quantity: v.QUANTITY_M6, price: v.PRICE_M6, amount: v.AMOUNT_M6),
                    HandleValueBaseString(quantity: v.QUANTITY_M7, price: v.PRICE_M7, amount: v.AMOUNT_M7),
                    HandleValueBaseString(quantity: v.QUANTITY_M8, price: v.PRICE_M8, amount: v.AMOUNT_M8),
                    HandleValueBaseString(quantity: v.QUANTITY_M9, price: v.PRICE_M9, amount: v.AMOUNT_M9),
                    HandleValueBaseString(quantity: v.QUANTITY_M10, price: v.PRICE_M10, amount: v.AMOUNT_M10),
                    HandleValueBaseString(quantity: v.QUANTITY_M11, price: v.PRICE_M11, amount: v.AMOUNT_M11),
                    HandleValueBaseString(quantity: v.QUANTITY_M12, price: v.PRICE_M12, amount: v.AMOUNT_M12),
                    HandleValueBaseString(quantity: v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1, price: null, amount: v.AMOUNT_YEAR),

                    HandleValueBaseString(quantity: null, price: null, amount: v.AMOUNT_YEAR_PREVENTIVE),

                    HandleValueBaseString(quantity: (v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1)/12, price: null, amount: v.AMOUNT_YEAR/12),
                },
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_OTHER_COST_PL_DATA_BASE v)
        {
            return new T_MD_COST_PL_ELEMENT
            {
                ORG_NAME = $"{v.MATERIAL} ({v.UNIT})",
                TEMPLATE_CODE = v.TEMPLATE_CODE + v.OTHER_PROFIT_CENTER_CODE,
                TEMPLATE_CODE_PURE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                IS_GROUP = false,
                CENTER_CODE = v.ORG_CODE,
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template,
                IsBase = true,
                ValuesBaseString = new string[15]
                {
                    HandleValueBaseString(quantity: v.QUANTITY_M1, price: v.PRICE_M1, amount: v.AMOUNT_M1),
                    HandleValueBaseString(quantity: v.QUANTITY_M2, price: v.PRICE_M2, amount: v.AMOUNT_M2),
                    HandleValueBaseString(quantity: v.QUANTITY_M3, price: v.PRICE_M3, amount: v.AMOUNT_M3),
                    HandleValueBaseString(quantity: v.QUANTITY_M4, price: v.PRICE_M4, amount: v.AMOUNT_M4),
                    HandleValueBaseString(quantity: v.QUANTITY_M5, price: v.PRICE_M5, amount: v.AMOUNT_M5),
                    HandleValueBaseString(quantity: v.QUANTITY_M6, price: v.PRICE_M6, amount: v.AMOUNT_M6),
                    HandleValueBaseString(quantity: v.QUANTITY_M7, price: v.PRICE_M7, amount: v.AMOUNT_M7),
                    HandleValueBaseString(quantity: v.QUANTITY_M8, price: v.PRICE_M8, amount: v.AMOUNT_M8),
                    HandleValueBaseString(quantity: v.QUANTITY_M9, price: v.PRICE_M9, amount: v.AMOUNT_M9),
                    HandleValueBaseString(quantity: v.QUANTITY_M10, price: v.PRICE_M10, amount: v.AMOUNT_M10),
                    HandleValueBaseString(quantity: v.QUANTITY_M11, price: v.PRICE_M11, amount: v.AMOUNT_M11),
                    HandleValueBaseString(quantity: v.QUANTITY_M12, price: v.PRICE_M12, amount: v.AMOUNT_M12),
                    HandleValueBaseString(quantity: v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1, price: null, amount: v.AMOUNT_YEAR),

                    HandleValueBaseString(quantity: null, price: null, amount: v.AMOUNT_YEAR_PREVENTIVE),

                    HandleValueBaseString(quantity: (v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1)/12, price: null, amount: v.AMOUNT_YEAR/12),
                },
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_OTHER_COST_PL_DATA_BASE_HISTORY v)
        {
            return new T_MD_COST_PL_ELEMENT
            {
                ORG_NAME = $"{v.MATERIAL} ({v.UNIT})",
                TEMPLATE_CODE = v.TEMPLATE_CODE + v.OTHER_PROFIT_CENTER_CODE,
                TEMPLATE_CODE_PURE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                CENTER_CODE = v.ORG_CODE,
                VERSION = v.VERSION,
                IS_GROUP = false,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template,
                IsBase = true,
                ValuesBaseString = new string[15]
                {
                    HandleValueBaseString(quantity: v.QUANTITY_M1, price: v.PRICE_M1, amount: v.AMOUNT_M1),
                    HandleValueBaseString(quantity: v.QUANTITY_M2, price: v.PRICE_M2, amount: v.AMOUNT_M2),
                    HandleValueBaseString(quantity: v.QUANTITY_M3, price: v.PRICE_M3, amount: v.AMOUNT_M3),
                    HandleValueBaseString(quantity: v.QUANTITY_M4, price: v.PRICE_M4, amount: v.AMOUNT_M4),
                    HandleValueBaseString(quantity: v.QUANTITY_M5, price: v.PRICE_M5, amount: v.AMOUNT_M5),
                    HandleValueBaseString(quantity: v.QUANTITY_M6, price: v.PRICE_M6, amount: v.AMOUNT_M6),
                    HandleValueBaseString(quantity: v.QUANTITY_M7, price: v.PRICE_M7, amount: v.AMOUNT_M7),
                    HandleValueBaseString(quantity: v.QUANTITY_M8, price: v.PRICE_M8, amount: v.AMOUNT_M8),
                    HandleValueBaseString(quantity: v.QUANTITY_M9, price: v.PRICE_M9, amount: v.AMOUNT_M9),
                    HandleValueBaseString(quantity: v.QUANTITY_M10, price: v.PRICE_M10, amount: v.AMOUNT_M10),
                    HandleValueBaseString(quantity: v.QUANTITY_M11, price: v.PRICE_M11, amount: v.AMOUNT_M11),
                    HandleValueBaseString(quantity: v.QUANTITY_M12, price: v.PRICE_M12, amount: v.AMOUNT_M12),
                    HandleValueBaseString(quantity: v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1, price: null, amount: v.AMOUNT_YEAR),
                    HandleValueBaseString(quantity: null, price: null, amount: v.AMOUNT_YEAR_PREVENTIVE),

                    HandleValueBaseString(quantity: (v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1)/12, price: null, amount: v.AMOUNT_YEAR/12),
                },
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_CONTRUCT_COST_PL_DATA_BASE v)
        {
            return new T_MD_COST_PL_ELEMENT
            {
                ORG_NAME = $"{v.MATERIAL} ({v.UNIT})",
                TEMPLATE_CODE = v.TEMPLATE_CODE + v.INTERNAL_ORDER_CODE,
                TEMPLATE_CODE_PURE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                IS_GROUP = false,
                CENTER_CODE = v.ORG_CODE,
                VERSION = v.VERSION,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template,
                IsBase = true,
                ValuesBaseString = new string[15]
                {
                    HandleValueBaseString(quantity: v.QUANTITY_M1, price: v.PRICE_M1, amount: v.AMOUNT_M1),
                    HandleValueBaseString(quantity: v.QUANTITY_M2, price: v.PRICE_M2, amount: v.AMOUNT_M2),
                    HandleValueBaseString(quantity: v.QUANTITY_M3, price: v.PRICE_M3, amount: v.AMOUNT_M3),
                    HandleValueBaseString(quantity: v.QUANTITY_M4, price: v.PRICE_M4, amount: v.AMOUNT_M4),
                    HandleValueBaseString(quantity: v.QUANTITY_M5, price: v.PRICE_M5, amount: v.AMOUNT_M5),
                    HandleValueBaseString(quantity: v.QUANTITY_M6, price: v.PRICE_M6, amount: v.AMOUNT_M6),
                    HandleValueBaseString(quantity: v.QUANTITY_M7, price: v.PRICE_M7, amount: v.AMOUNT_M7),
                    HandleValueBaseString(quantity: v.QUANTITY_M8, price: v.PRICE_M8, amount: v.AMOUNT_M8),
                    HandleValueBaseString(quantity: v.QUANTITY_M9, price: v.PRICE_M9, amount: v.AMOUNT_M9),
                    HandleValueBaseString(quantity: v.QUANTITY_M10, price: v.PRICE_M10, amount: v.AMOUNT_M10),
                    HandleValueBaseString(quantity: v.QUANTITY_M11, price: v.PRICE_M11, amount: v.AMOUNT_M11),
                    HandleValueBaseString(quantity: v.QUANTITY_M12, price: v.PRICE_M12, amount: v.AMOUNT_M12),
                    HandleValueBaseString(quantity: v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1, price: null, amount: v.AMOUNT_YEAR),

                    HandleValueBaseString(quantity: null, price: null, amount: v.AMOUNT_YEAR_PREVENTIVE),

                    HandleValueBaseString(quantity: (v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1)/12, price: null, amount: v.AMOUNT_YEAR/12),
                },
            };
        }

        public static explicit operator T_MD_COST_PL_ELEMENT(T_BP_CONTRUCT_COST_PL_DATA_BASE_HISTORY v)
        {
            return new T_MD_COST_PL_ELEMENT
            {
                ORG_NAME = $"{v.MATERIAL} ({v.UNIT})",
                TEMPLATE_CODE = v.TEMPLATE_CODE + v.INTERNAL_ORDER_CODE,
                TEMPLATE_CODE_PURE = v.TEMPLATE_CODE,
                ORG_CODE = v.ORG_CODE,
                CENTER_CODE = v.ORG_CODE,
                VERSION = v.VERSION,
                IS_GROUP = false,
                DESCRIPTION = v.DESCRIPTION,
                Template = v.Template,
                IsBase = true,
                ValuesBaseString = new string[15]
                {
                    HandleValueBaseString(quantity: v.QUANTITY_M1, price: v.PRICE_M1, amount: v.AMOUNT_M1),
                    HandleValueBaseString(quantity: v.QUANTITY_M2, price: v.PRICE_M2, amount: v.AMOUNT_M2),
                    HandleValueBaseString(quantity: v.QUANTITY_M3, price: v.PRICE_M3, amount: v.AMOUNT_M3),
                    HandleValueBaseString(quantity: v.QUANTITY_M4, price: v.PRICE_M4, amount: v.AMOUNT_M4),
                    HandleValueBaseString(quantity: v.QUANTITY_M5, price: v.PRICE_M5, amount: v.AMOUNT_M5),
                    HandleValueBaseString(quantity: v.QUANTITY_M6, price: v.PRICE_M6, amount: v.AMOUNT_M6),
                    HandleValueBaseString(quantity: v.QUANTITY_M7, price: v.PRICE_M7, amount: v.AMOUNT_M7),
                    HandleValueBaseString(quantity: v.QUANTITY_M8, price: v.PRICE_M8, amount: v.AMOUNT_M8),
                    HandleValueBaseString(quantity: v.QUANTITY_M9, price: v.PRICE_M9, amount: v.AMOUNT_M9),
                    HandleValueBaseString(quantity: v.QUANTITY_M10, price: v.PRICE_M10, amount: v.AMOUNT_M10),
                    HandleValueBaseString(quantity: v.QUANTITY_M11, price: v.PRICE_M11, amount: v.AMOUNT_M11),
                    HandleValueBaseString(quantity: v.QUANTITY_M12, price: v.PRICE_M12, amount: v.AMOUNT_M12),
                    HandleValueBaseString(quantity: v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1, price: null, amount: v.AMOUNT_YEAR),
                    HandleValueBaseString(quantity: null, price: null, amount: v.AMOUNT_YEAR_PREVENTIVE),

                    HandleValueBaseString(quantity: (v.QUANTITY_M12 +v.QUANTITY_M11 +v.QUANTITY_M10 +v.QUANTITY_M9 +v.QUANTITY_M8 +v.QUANTITY_M7 +v.QUANTITY_M6 +v.QUANTITY_M5 +v.QUANTITY_M4 +v.QUANTITY_M3 +v.QUANTITY_M2 + v.QUANTITY_M1)/12, price: null, amount: v.AMOUNT_YEAR/12),
                },
            };
        }

    }
}
