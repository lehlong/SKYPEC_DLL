using SharpSapRfc;

using System;

namespace SMO.Core.Entities.MD
{
    public class T_SAP_ACTUAL_DATA : BaseEntity
    {
        [RfcStructureField("BUZEI")]
        public virtual string LINE_NUMBER { get; set; }
        [RfcStructureField("BUKRS")]
        public virtual string COMPANY_CODE { get; set; }
        [RfcStructureField("BELNR")]
        public virtual string DOCUMENT_NUMBER { get; set; }
        [RfcStructureField("GJAHR")]
        public virtual int FISCAL_YEAR { get; set; }
        [RfcStructureField("H_BUDAT")]
        public virtual DateTime? POSTING_DATE { get; set; }
        [RfcStructureField("BSCHL")]
        public virtual string POSTING_KEY { get; set; }
        [RfcStructureField("SHKZG")]
        public virtual string DEBIT_CREDIT { get; set; }
        [RfcStructureField("MWSKZ")]
        public virtual string TAX_CODE { get; set; }
        [RfcStructureField("HKONT")]
        public virtual string GL_ACCOUNT { get; set; }
        [RfcStructureField("PSWBT")]
        public virtual Decimal GL_AMOUNT { get; set; }
        [RfcStructureField("PSWSL")]
        public virtual string CURRENCY { get; set; }
        [RfcStructureField("WMWST")]
        public virtual Decimal TAX_AMOUNT { get; set; }
        [RfcStructureField("KOSTL")]
        public virtual string COST_CENTER { get; set; }
        [RfcStructureField("AUFNR")]
        public virtual string C_ORDER { get; set; }
        [RfcStructureField("PRCTR")]
        public virtual string PROFIT_CENTER { get; set; }
        [RfcStructureField("XREF3")]
        public virtual string FUND_CODE { get; set; }
        [RfcStructureField("XREF1")]
        public virtual string PAYMENT_CODE { get; set; }
        [RfcStructureField("XREF2")]
        public virtual string EXPENSE_CODE { get; set; }
        [RfcStructureField("CPUDT")]
        public virtual DateTime? CREATE_DATE_SAP { get; set; }
        [RfcStructureField("AEDAT")]
        public virtual DateTime? UPDATE_DATE_SAP { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is T_SAP_ACTUAL_DATA other)) return false;
            return ReferenceEquals(this, other) ? true : COMPANY_CODE == other.COMPANY_CODE &&
                DOCUMENT_NUMBER == other.DOCUMENT_NUMBER && FISCAL_YEAR == other.FISCAL_YEAR && LINE_NUMBER == other.LINE_NUMBER;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ COMPANY_CODE.GetHashCode();
                hash = (hash * 31) ^ DOCUMENT_NUMBER.GetHashCode();
                hash = (hash * 31) ^ FISCAL_YEAR.GetHashCode();
                hash = (hash * 31) ^ LINE_NUMBER.GetHashCode();
                return hash;
            }
        }
    }
}
