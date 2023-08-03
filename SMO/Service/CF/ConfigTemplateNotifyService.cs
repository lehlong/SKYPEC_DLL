using SMO.Core.Entities;
using SMO.Repository.Implement.CF;

using System;

namespace SMO.Service.CF
{
    public class ConfigTemplateNotifyService : GenericService<T_CF_TEMPLATE_NOTIFY, ConfigTemplateNotifyRepo>
    {
        public ConfigTemplateNotifyService() : base()
        {
        }

        public void GetTemplate()
        {
            GetAll();
            if (ObjList == null || ObjList.Count == 0 && State)
            {
                ObjDetail = new T_CF_TEMPLATE_NOTIFY
                {
                    PKID = Guid.NewGuid().ToString()
                };
                Create();
            }
            else
            {
                ObjDetail = ObjList[0];
            }
        }
    }
}