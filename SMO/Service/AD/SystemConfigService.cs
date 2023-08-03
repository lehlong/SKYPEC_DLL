using SMO.Core.Entities;
using SMO.Repository.Implement.AD;

using System;

namespace SMO.Service.AD
{
    public class SystemConfigService : GenericService<T_AD_SYSTEM_CONFIG, SystemConfigRepo>
    {
        public SystemConfigService() : base()
        {

        }

        public void GetConfig()
        {
            GetAll();
            if (ObjList.Count == 0 && State)
            {
                ObjDetail = new T_AD_SYSTEM_CONFIG
                {
                    PKID = Guid.NewGuid().ToString()
                };
                Create();
            }
            else
            {
                ObjDetail = ObjList[0];
                //var connection = ObjList[0].Connection;
            }
        }

        public override void Update()
        {
            try
            {
                base.Update();
                if (State)
                {
                    //SAPINT.SAPDestitination.Init(this.ObjDetail.SAP_HOST, this.ObjDetail.SAP_CLIENT, this.ObjDetail.SAP_NUMBER);
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
