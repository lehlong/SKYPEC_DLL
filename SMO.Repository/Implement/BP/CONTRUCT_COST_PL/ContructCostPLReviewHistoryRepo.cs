﻿using SMO.Core.Entities.BP.COST_PL;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_PL;

namespace SMO.Repository.Implement.BP.COST_PL
{
    public class ContructCostPLReviewHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_REVIEW_HISTORY>, IContructCostPLReviewHistoryRepo
    {
        public ContructCostPLReviewHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}