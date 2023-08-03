using SharpSapRfc;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using System.Collections.Generic;

namespace SMO.SAPINT.Function
{
    public class Get_ActualData_Function : RfcFunctionObject<IEnumerable<T_SAP_ACTUAL_DATA>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_ACTUAL"; }
        }

        public override IEnumerable<T_SAP_ACTUAL_DATA> GetOutput(RfcResult result)
        {
            return result.GetTable<T_SAP_ACTUAL_DATA>("T_ACTUAL");
        }
    }
}
