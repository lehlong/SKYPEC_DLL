using SMO.Core.Entities.BP.REVENUE_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.REVENUE_CF;

using System;
using System.Collections.Generic;

namespace SMO.Repository.Implement.BP.REVENUE_CF
{
    public class RevenueCFSumUpDetailRepo : GenericRepository<T_BP_REVENUE_CF_SUM_UP_DETAIL>, IRevenueCFSumUpDetailRepo
    {
        public RevenueCFSumUpDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_BP_REVENUE_CF_SUM_UP_DETAIL> Search(T_BP_REVENUE_CF_SUM_UP_DETAIL objFilter, int pageSize, int pageIndex, out int total)
        {
            throw new NotImplementedException();
        }
    }
}
