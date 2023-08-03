using SharpSapRfc;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.SAPINT.Class;
using System.Collections.Generic;

namespace SMO.SAPINT.Function
{
    public class SynMD_Expense_Function : RfcFunctionObject<IEnumerable<T_SAP_MD_EXPENSE>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_GET_MAPHI"; }
        }

        public override IEnumerable<T_SAP_MD_EXPENSE> GetOutput(RfcResult result)
        {
            return result.GetTable<T_SAP_MD_EXPENSE>("T_MAPHI");
        }
    }

    public class SynMD_InternalOrder_Function : RfcFunctionObject<IEnumerable<T_MD_INTERNAL_ORDER>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_GET_INTERNAL"; }
        }

        public override IEnumerable<T_MD_INTERNAL_ORDER> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_INTERNAL_ORDER>("T_INTERNAL");
        }
    }

    public class SynMD_InternalOrder_Header_Function : RfcFunctionObject<IEnumerable<ZST_SETHEADER>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_SETHEADER"; }
        }

        public override IEnumerable<ZST_SETHEADER> GetOutput(RfcResult result)
        {
            return result.GetTable<ZST_SETHEADER>("T_SETHEADER");
        }
    }

    public class SynMD_ProfitCenter_Function : RfcFunctionObject<IEnumerable<T_MD_PROFIT_CENTER>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_GET_PROFIT"; }
        }

        public override IEnumerable<T_MD_PROFIT_CENTER> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_PROFIT_CENTER>("T_PROFIT");
        }
    }

    public class SynMD_Company_Function : RfcFunctionObject<IEnumerable<T_MD_COMPANY>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_COMPANYCODE"; }
        }

        public override IEnumerable<T_MD_COMPANY> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_COMPANY>("T_BUKRS");
        }
    }

    public class SynMD_Project_Function : RfcFunctionObject<IEnumerable<T_MD_PROJECT>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_GET_PROJECT"; }
        }

        public override IEnumerable<T_MD_PROJECT> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_PROJECT>("T_PROJECT");
        }
    }

    public class SynMD_GLAccount_Function : RfcFunctionObject<IEnumerable<T_SAP_MD_GLACCOUNT>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_GET_GLACCOUNT"; }
        }

        public override IEnumerable<T_SAP_MD_GLACCOUNT> GetOutput(RfcResult result)
        {
            return result.GetTable<T_SAP_MD_GLACCOUNT>("T_GLACCOUNT");
        }
    }

    public class SynMD_Payment_Function : RfcFunctionObject<IEnumerable<T_SAP_MD_PAYMENT>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_GET_MATHUCHI"; }
        }

        public override IEnumerable<T_SAP_MD_PAYMENT> GetOutput(RfcResult result)
        {
            return result.GetTable<T_SAP_MD_PAYMENT>("T_MATHUCHI");
        }
    }
}
