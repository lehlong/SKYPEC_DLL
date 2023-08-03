using SMO.Core.Entities;
using SMO.Repository.Implement.MD;

using System;

namespace SMO.Service.MD
{
    public class DomainService : GenericService<T_MD_DOMAIN, DomainRepo>
    {
        public DomainService() : base()
        {

        }

        public override void Create()
        {
            try
            {
                ObjDetail.ACTIVE = true;
                if (!CheckExist(x => x.CODE == ObjDetail.CODE))
                {
                    base.Create();
                }
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }
        }
    }
}
