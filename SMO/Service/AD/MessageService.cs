using SMO.Core.Entities;
using SMO.Repository.Implement.AD;

using System;

namespace SMO.Service.AD
{
    public class MessageService : GenericService<T_AD_MESSAGE, MessageRepo>
    {
        public MessageService() : base()
        {

        }

        public void Update(string message, string id)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                Get(id);
                ObjDetail.MESSAGE = message.Trim();
                if (ProfileUtilities.User != null)
                {
                    ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    ObjDetail.UPDATE_DATE = CurrentRepository.GetDateDatabase();
                }
                CurrentRepository.Update(ObjDetail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public override void Create()
        {
            try
            {
                if (!CheckExist(x => x.CODE == ObjDetail.CODE && x.LANGUAGE == ObjDetail.LANGUAGE))
                {
                    ObjDetail.PKID = Guid.NewGuid().ToString();
                    ObjDetail.ACTIVE = true;
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
