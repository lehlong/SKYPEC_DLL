using SMO.Core.Entities;
using SMO.Repository.Implement.CM;

namespace SMO.Service.CM
{
    public class SmsNotifyService : GenericService<T_CM_SMS, SmsRepo>
    {
        public SmsNotifyService() : base()
        {

        }

        public void Reset(string id)
        {
            Get(id);
            ObjDetail.NUMBER_RETRY = 0;
            Update();
        }
    }
}
