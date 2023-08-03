using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

using System;
using System.Collections.Generic;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class ContructCostCFSumUpDetailRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_SUM_UP_DETAIL>, IContructCostCFSumUpDetailRepo
    {
        public ContructCostCFSumUpDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_BP_CONTRUCT_COST_CF_SUM_UP_DETAIL> Search(T_BP_CONTRUCT_COST_CF_SUM_UP_DETAIL objFilter, int pageSize, int pageIndex, out int total)
        {
            throw new NotImplementedException();
        }
    }
}
