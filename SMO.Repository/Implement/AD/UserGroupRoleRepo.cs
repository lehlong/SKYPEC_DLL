﻿using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

namespace SMO.Repository.Implement.AD
{
    public class UserGroupRoleRepo : GenericRepository<T_AD_USER_GROUP_ROLE>, IUserGroupRoleRepo
    {
        public UserGroupRoleRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
