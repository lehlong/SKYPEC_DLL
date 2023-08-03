﻿using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

namespace SMO.Repository.Implement.AD
{
    public class UserRightRepo : GenericRepository<T_AD_USER_RIGHT>, IUserRightRepo
    {
        public UserRightRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
