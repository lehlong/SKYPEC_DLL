using SMO.Core.Entities;
using SMO.Repository.Implement.MD;

using System;

namespace SMO.Service.MD
{
    public class DictionaryService : GenericService<T_MD_DICTIONARY, DictionaryRepo>
    {
        public DictionaryService() : base()
        {

        }

        public override void Create()
        {
            try
            {
                if (!CheckExist(x => x.CODE == ObjDetail.CODE && x.LANG == ObjDetail.LANG && x.FK_DOMAIN == ObjDetail.FK_DOMAIN))
                {
                    ObjDetail.ACTIVE = true;
                    ObjDetail.PKID = Guid.NewGuid().ToString();
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
