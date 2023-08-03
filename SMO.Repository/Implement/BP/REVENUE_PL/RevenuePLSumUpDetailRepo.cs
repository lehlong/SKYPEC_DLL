using SMO.Core.Entities.BP.REVENUE_PL;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.REVENUE_PL;

using System;
using System.Collections.Generic;

namespace SMO.Repository.Implement.BP.REVENUE_PL
{
    public class RevenuePLSumUpDetailRepo : GenericRepository<T_BP_REVENUE_PL_SUM_UP_DETAIL>, IRevenuePLSumUpDetailRepo
    {
        public RevenuePLSumUpDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_BP_REVENUE_PL_SUM_UP_DETAIL> Search(T_BP_REVENUE_PL_SUM_UP_DETAIL objFilter, int pageSize, int pageIndex, out int total)
        {
            throw new NotImplementedException();
        }
    }
}
