﻿using SMO.Core.Entities;
using SMO.Repository.Implement.MD;

using System;

namespace SMO.Service.MD
{
    public class BidTypeService : GenericService<T_MD_BID_TYPE, BidTypeRepo>
    {
        public BidTypeService() : base()
        {

        }

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
