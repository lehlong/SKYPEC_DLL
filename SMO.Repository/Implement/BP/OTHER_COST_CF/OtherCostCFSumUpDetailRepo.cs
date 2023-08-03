using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

using System;
using System.Collections.Generic;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class OtherCostCFSumUpDetailRepo : GenericRepository<T_BP_OTHER_COST_CF_SUM_UP_DETAIL>, IOtherCostCFSumUpDetailRepo
    {
        public OtherCostCFSumUpDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_BP_OTHER_COST_CF_SUM_UP_DETAIL> Search(T_BP_OTHER_COST_CF_SUM_UP_DETAIL objFilter, int pageSize, int pageIndex, out int total)
        {
            throw new NotImplementedException();
        }
    }
}
