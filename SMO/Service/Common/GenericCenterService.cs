using SMO.Core.Common;
using SMO.Repository.Common;

using System;

namespace SMO.Service.Common
{
    public class GenericCenterService<T, Repo> : GenericService<T, Repo> where T : CoreCenter where Repo : GenericCenterRepository<T>
    {
        public override void Create()
        {
            try
            {
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
