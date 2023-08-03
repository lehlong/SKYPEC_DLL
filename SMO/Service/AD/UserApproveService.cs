using SMO.Core.Entities;
using SMO.Repository.Implement.AD;

using System;
using System.Linq;

namespace SMO.Service.AD
{
    public class UserApproveService : GenericService<T_AD_USER_APPROVE, UserApproveRepo>
    {
        public void Update(string userApprove, string modul)
        {
            try
            {
                var find = CurrentRepository.Queryable().FirstOrDefault(x => x.USER_NAME == ProfileUtilities.User.USER_NAME && x.MODUL == modul);

                UnitOfWork.BeginTransaction();
                if (find == null)
                {
                    CurrentRepository.Create(new T_AD_USER_APPROVE()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        USER_NAME = ProfileUtilities.User.USER_NAME,
                        MODUL = modul,
                        USER_APPROVE = userApprove
                    });
                }
                else
                {
                    find.USER_APPROVE = userApprove;
                    CurrentRepository.Update(find);
                }
                UnitOfWork.Commit();
            }
            catch
            {
                UnitOfWork.Rollback();
            }
        }

        public string GetUserApprove(string modul)
        {
            var find = CurrentRepository.Queryable().FirstOrDefault(x => x.USER_NAME == ProfileUtilities.User.USER_NAME && x.MODUL == modul);
            if (find != null)
            {
                return find.USER_APPROVE;
            }
            return "";
        }
    }
}
