using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_SAP_ACTUAL_DATA_Map : BaseMapping<T_SAP_ACTUAL_DATA>
    {
        public T_SAP_ACTUAL_DATA_Map()
        {
            Table("T_SAP_ACTUAL_DATA");
            CompositeId()
                .KeyProperty(x => x.COMPANY_CODE)
                .KeyProperty(x => x.DOCUMENT_NUMBER)
                .KeyProperty(x => x.FISCAL_YEAR)
                .KeyProperty(x => x.LINE_NUMBER);
            Map(x => x.POSTING_DATE);
            Map(x => x.POSTING_KEY);
            Map(x => x.DEBIT_CREDIT);
            Map(x => x.TAX_CODE);
            Map(x => x.GL_ACCOUNT);
            Map(x => x.GL_AMOUNT);
            Map(x => x.CURRENCY);
            Map(x => x.TAX_AMOUNT);
            Map(x => x.COST_CENTER);
            Map(x => x.C_ORDER);
            Map(x => x.PROFIT_CENTER);
            Map(x => x.FUND_CODE);
            Map(x => x.PAYMENT_CODE);
            Map(x => x.EXPENSE_CODE);
            Map(x => x.CREATE_DATE_SAP);
            Map(x => x.UPDATE_DATE_SAP);
        }
    }
}
