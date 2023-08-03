using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.MD
{
    public class UserReviewService : GenericService<T_MD_USER_REVIEW, UserReviewRepo>
    {
        internal IList<int> GetPeriodTime()
        {
            return UnitOfWork.Repository<PeriodTimeRepo>()
                .GetAll()
                .Select(x => x.TIME_YEAR)
                .OrderByDescending(x => x)
                .ToList();
        }

        public void Create(int year, IList<string> userNames)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                foreach (var userName in userNames)
                {
                    if (!CheckExist(x => x.TIME_YEAR == year && x.USER_NAME == userName))
                    {
                        CurrentRepository.Create(new T_MD_USER_REVIEW
                        {
                            TIME_YEAR = year,
                            USER_NAME = userName,
                            PKID = Guid.NewGuid().ToString(),
                            CREATE_BY = ProfileUtilities.User?.USER_NAME
                        });
                    }
                    else
                    {
                        UnitOfWork.Rollback();
                        State = false;
                        MesseageCode = "1101";
                        return;
                    }
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        internal void DeleteEntity(string userName, int year)
        {
            var entity = GetFirstWithFetch(x => x.TIME_YEAR == year && x.USER_NAME == userName);
            if (entity == null)
            {
                State = false;
                ErrorMessage = "Không tìm thấy người thẩm định này";
            }
            else
            {
                try
                {
                    UnitOfWork.BeginTransaction();

                    CurrentRepository.Delete(entity);

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
}
