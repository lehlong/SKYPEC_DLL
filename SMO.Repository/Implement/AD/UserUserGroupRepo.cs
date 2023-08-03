﻿using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

namespace SMO.Repository.Implement.AD
{
    public class UserUserGroupRepo : GenericRepository<T_AD_USER_USER_GROUP>, IUserUserGroupRepo
    {
        public UserUserGroupRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
