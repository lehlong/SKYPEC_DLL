using SMO.Core.Entities.BP;
using SMO.Repository.Implement.BP;

using System;
using System.Linq;

namespace SMO.Service.BP
{
    public class RegisterService : GenericService<T_BP_REGISTER, RegisterRepo>
    {
        internal void GetMyRegisters()
        {
            var myOrg = ProfileUtilities.User.ORGANIZE_CODE;
            ObjList = CurrentRepository
                .GetManyWithFetch(x => x.ORG_CODE == myOrg && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.BpType.ACTIVE, x => x.BpType, x => x.Organize)
                .ToList();
            var bpTypes = UnitOfWork.Repository<TypeRepo>().GetManyByExpression(x => x.ACTIVE == true);
            if (ObjList == null || ObjList.Count == 0)
            {
                // user not register yet
                ObjList = (from type in bpTypes
                           select new T_BP_REGISTER
                           {
                               ID = Guid.NewGuid(),
                               TYPE_ID = type.ID,
                               IS_REGISTER = true,
                               TIME_YEAR = ObjDetail.TIME_YEAR,
                               BpType = type,
                               ORG_CODE = myOrg
                           }).ToList();
            }
            else if (ObjList.Count < bpTypes.Count)
            {
                ObjList.AddRange(from type in bpTypes.Where(x => !ObjList.Select(y => y.TYPE_ID).Contains(x.ID))
                                 select new T_BP_REGISTER
                                 {
                                     ID = Guid.NewGuid(),
                                     TYPE_ID = type.ID,
                                     IS_REGISTER = true,
                                     TIME_YEAR = ObjDetail.TIME_YEAR,
                                     BpType = type,
                                     ORG_CODE = myOrg
                                 });
            }
        }

        internal void SaveRegister()
        {
            try
            {
                var myOrg = ProfileUtilities.User.ORGANIZE_CODE;
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var registed = GetManyWithFetch(x => x.TIME_YEAR == ObjList.FirstOrDefault().TIME_YEAR && x.ORG_CODE == myOrg);
                foreach (var item in registed)
                {
                    CurrentRepository.Detach(item);
                }
                UnitOfWork.BeginTransaction();
                if (registed.Count() > 0)
                {
                    foreach (var item in ObjList.Where(x => !registed
                    .Select(y => y.TYPE_ID).Contains(x.TYPE_ID)))
                    {
                        item.CREATE_BY = currentUser;
                        CurrentRepository.Create(item);
                    }
                    foreach (var item in ObjList.Where(x => registed
                    .Select(y => y.TYPE_ID).Contains(x.TYPE_ID)))
                    {
                        item.UPDATE_BY = currentUser;
                        CurrentRepository.Update(item);
                    }
                }
                else
                {
                    ObjList.ForEach(x => x.CREATE_BY = currentUser);
                    CurrentRepository.Create(ObjList);
                }
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                ErrorMessage = e.Message;
            }

        }
    }
}
