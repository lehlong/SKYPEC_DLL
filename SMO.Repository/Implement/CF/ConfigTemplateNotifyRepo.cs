﻿using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;

namespace SMO.Repository.Implement.CF
{
    public class ConfigTemplateNotifyRepo : GenericRepository<T_CF_TEMPLATE_NOTIFY>, IConfigTemplateNotify
    {
        public ConfigTemplateNotifyRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
