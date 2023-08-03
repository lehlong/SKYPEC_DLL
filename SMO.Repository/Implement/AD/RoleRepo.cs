﻿using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class RoleRepo : GenericRepository<T_AD_ROLE>, IRoleRepo
    {
        public RoleRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_AD_ROLE> Search(T_AD_ROLE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(objFilter.CODE.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(objFilter.NAME.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.NOTES))
            {
                query = query.Where(x => x.NOTES.ToLower().Contains(objFilter.NOTES.ToLower()));
            }

            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
