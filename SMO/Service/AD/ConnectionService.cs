using SMO.Core.Entities;
using SMO.Repository.Implement.AD;

using System;

namespace SMO.Service.AD
{
    public class ConnectionService : GenericService<T_AD_CONNECTION, ConnectionRepo>
    {
        public ConnectionService() : base()
        {

        }

        public override void Create()
        {
            ObjDetail.PKID = Guid.NewGuid().ToString();
            base.Create();
        }
    }
}
