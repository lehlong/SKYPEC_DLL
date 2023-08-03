using SMO.Core.Entities;
using SMO.Repository.Implement.CM;

using System;

namespace SMO.Service.CM
{
    public class EmailNotifyService : GenericService<T_CM_EMAIL, EmailRepo>
    {
        public EmailNotifyService() : base()
        {

        }

        public void Reset(string id)
        {
            Get(id);

            var newEmail = new T_CM_EMAIL()
            {
                PKID = Guid.NewGuid().ToString(),
                CONTENTS = ObjDetail.CONTENTS,
                SUBJECT = ObjDetail.SUBJECT,
                IS_SEND = false,
                EMAIL = ObjDetail.EMAIL,
                NUMBER_RETRY = 0,
                CREATE_BY = ProfileUtilities.User.USER_NAME
            };

            try
            {
                UnitOfWork.BeginTransaction();
                CurrentRepository.Create(newEmail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }
    }
}
