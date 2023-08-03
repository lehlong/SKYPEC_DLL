﻿using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class ContructCostCFReviewHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_REVIEW_HISTORY>, IContructCostCFReviewHistoryRepo
    {
        public ContructCostCFReviewHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
