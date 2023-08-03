using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;

namespace SMO.Repository.Implement.CM
{
    public class SmsOTPRepo : GenericRepository<T_CM_SMS_OTP>, ISmsOTPRepo
    {
        public SmsOTPRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
