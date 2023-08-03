using SMO.Core.Entities.BP.COST_PL;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_PL;

using System;
using System.Collections.Generic;

namespace SMO.Repository.Implement.BP.COST_PL
{
    public class ContructCostPLSumUpDetailRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_SUM_UP_DETAIL>, IContructCostPLSumUpDetailRepo
    {
        public ContructCostPLSumUpDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_BP_CONTRUCT_COST_PL_SUM_UP_DETAIL> Search(T_BP_CONTRUCT_COST_PL_SUM_UP_DETAIL objFilter, int pageSize, int pageIndex, out int total)
        {
            throw new NotImplementedException();
        }
    }
}
