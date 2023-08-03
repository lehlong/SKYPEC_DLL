using SMO.Core.Entities.BP;
using SMO.Repository.Common;

namespace SMO.Repository.Implement.BP
{
    public class StepperBudgetRepo : GenericRepository<T_BP_STEPPER_BUDGET>
    {
        public StepperBudgetRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
