using SMO.Core.Entities.MD;

namespace SMO.Core.Entities.BP.COST_PL.COST_PL_DATA_BASE
{
    public class T_BP_COST_PL_DATA_BASE : BaseEntity
    {
        public virtual string PKID { get; set; }

        public virtual string ORG_CODE { get; set; }
        public virtual string COST_CENTER_CODE { get; set; }
        public virtual string TEMPLATE_CODE { get; set; }
        public virtual string COST_PL_ELEMENT_CODE { get; set; }
        public virtual int VERSION { get; set; }
        public virtual int TIME_YEAR { get; set; }
        public virtual string MATERIAL { get; set; }
        public virtual string UNIT { get; set; }

        public virtual decimal QUANTITY_M1 { get; set; }
        public virtual decimal PRICE_M1 { get; set; }
        public virtual decimal AMOUNT_M1 { get; set; }

        public virtual decimal QUANTITY_M2 { get; set; }
        public virtual decimal PRICE_M2 { get; set; }
        public virtual decimal AMOUNT_M2 { get; set; }

        public virtual decimal QUANTITY_M3 { get; set; }
        public virtual decimal PRICE_M3 { get; set; }
        public virtual decimal AMOUNT_M3 { get; set; }

        public virtual decimal QUANTITY_M4 { get; set; }
        public virtual decimal PRICE_M4 { get; set; }
        public virtual decimal AMOUNT_M4 { get; set; }

        public virtual decimal QUANTITY_M5 { get; set; }
        public virtual decimal PRICE_M5 { get; set; }
        public virtual decimal AMOUNT_M5 { get; set; }

        public virtual decimal QUANTITY_M6 { get; set; }
        public virtual decimal PRICE_M6 { get; set; }
        public virtual decimal AMOUNT_M6 { get; set; }

        public virtual decimal QUANTITY_M7 { get; set; }
        public virtual decimal PRICE_M7 { get; set; }
        public virtual decimal AMOUNT_M7 { get; set; }

        public virtual decimal QUANTITY_M8 { get; set; }
        public virtual decimal PRICE_M8 { get; set; }
        public virtual decimal AMOUNT_M8 { get; set; }

        public virtual decimal QUANTITY_M9 { get; set; }
        public virtual decimal PRICE_M9 { get; set; }
        public virtual decimal AMOUNT_M9 { get; set; }

        public virtual decimal QUANTITY_M10 { get; set; }
        public virtual decimal PRICE_M10 { get; set; }
        public virtual decimal AMOUNT_M10 { get; set; }

        public virtual decimal QUANTITY_M11 { get; set; }
        public virtual decimal PRICE_M11 { get; set; }
        public virtual decimal AMOUNT_M11 { get; set; }

        public virtual decimal QUANTITY_M12 { get; set; }
        public virtual decimal PRICE_M12 { get; set; }
        public virtual decimal AMOUNT_M12 { get; set; }

        #region Ngân sách dự phòng
        public virtual decimal AMOUNT_YEAR_PREVENTIVE { get; set; }
        public virtual decimal AMOUNT_YEAR { get; set; }

        #endregion

        public virtual string DESCRIPTION { get; set; }

        public virtual T_MD_TEMPLATE Template { get; set; }
        public virtual T_MD_COST_PL_ELEMENT CostElement { get; set; }
        public virtual T_MD_COST_CENTER CostCenter { get; set; }
        public virtual T_MD_COST_CENTER Organize { get; set; }
    }
}
